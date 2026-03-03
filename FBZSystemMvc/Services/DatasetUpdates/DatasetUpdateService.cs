using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FBZ_System.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FBZSystemMvc.Services.DatasetUpdates;

public class DatasetUpdateService : IDatasetUpdateService
{
    private readonly IHttpClientFactory _http;
    private readonly IOptions<DatasetUpdateOptions> _options;
    private readonly ILogger<DatasetUpdateService> _log;
    private readonly IDatasetReloadable _reloadable;

    private readonly SemaphoreSlim _gate = new(1, 1);

    private string StatusPath => Path.Combine(_reloadable.DataFolderPath, "dataset_update_status.json");
    private string LocalCsvPath => Path.Combine(_reloadable.DataFolderPath, "names.csv");

    public DatasetUpdateService(
        IHttpClientFactory http,
        IOptions<DatasetUpdateOptions> options,
        ILogger<DatasetUpdateService> log,
        IDatasetReloadable reloadable)
    {
        _http = http;
        _options = options;
        _log = log;
        _reloadable = reloadable;
    }

    public DatasetUpdateStatus GetStatus()
    {
        var s = LoadStatus();
        s.RepositoryLoadedUtc = _reloadable.LoadedUtc;

        if (File.Exists(LocalCsvPath))
            s.LocalCsvLastWriteUtc = File.GetLastWriteTimeUtc(LocalCsvPath);

        return s;
    }

    public async Task<(bool Updated, string Message)> CheckAndUpdateAsync(bool force, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            Directory.CreateDirectory(_reloadable.DataFolderPath);

            var status = LoadStatus();
            status.LastCheckedUtc = DateTime.UtcNow;
            status.LastError = null;

            // Download ZIP to temp
            var zipTemp = Path.Combine(Path.GetTempPath(), $"fbz_dataset_{Guid.NewGuid():N}.zip");
            var csvTempDir = Path.Combine(Path.GetTempPath(), $"fbz_dataset_{Guid.NewGuid():N}");
            Directory.CreateDirectory(csvTempDir);
            var csvTempPath = Path.Combine(csvTempDir, "names.csv");

            try
            {
                var client = _http.CreateClient("dataset");
                using var resp = await client.GetAsync(_options.Value.ZipUrl, HttpCompletionOption.ResponseHeadersRead, ct);
                resp.EnsureSuccessStatusCode();

                await using (var fs = File.Create(zipTemp))
                await using (var rs = await resp.Content.ReadAsStreamAsync(ct))
                {
                    await rs.CopyToAsync(fs, ct);
                }

                var newHash = Sha256File(zipTemp);

                if (!force && !string.IsNullOrWhiteSpace(status.CurrentZipSha256) &&
                    string.Equals(status.CurrentZipSha256, newHash, StringComparison.OrdinalIgnoreCase))
                {
                    status.LastOutcome = "No change";
                    SaveStatus(status);
                    return (false, "No change detected (same ZIP hash).");
                }

                // Extract names.csv (case-insensitive)
                using (var zip = ZipFile.OpenRead(zipTemp))
                {
                    var entry = zip.Entries.FirstOrDefault(e =>
                        e.FullName.EndsWith(_options.Value.CsvFileName, StringComparison.OrdinalIgnoreCase));

                    if (entry == null)
                        throw new InvalidOperationException($"ZIP did not contain '{_options.Value.CsvFileName}'.");

                    entry.ExtractToFile(csvTempPath, overwrite: true);
                }

                // Validate by attempting to load it with your existing CSV repository
                // ComicRepositoryCsv expects names.csv in a folder.
                var validator = new ComicRepositoryCsv(csvTempDir);
                var count = validator.GetAllComics().Count;

                if (count < _options.Value.MinComicsExpected)
                    throw new InvalidOperationException($"Validation failed: only {count} comics loaded from new CSV.");

                // Swap in-place (backup old)
                if (File.Exists(LocalCsvPath))
                {
                    var backup = Path.Combine(_reloadable.DataFolderPath, $"names_backup_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
                    File.Copy(LocalCsvPath, backup, overwrite: true);
                }

                File.Copy(csvTempPath, LocalCsvPath, overwrite: true);

                // Hot reload in memory (no downtime)
                _reloadable.Reload();

                status.CurrentZipSha256 = newHash;
                status.LastUpdatedUtc = DateTime.UtcNow;
                status.LastOutcome = "Updated";
                status.LastError = null;
                SaveStatus(status);

                return (true, $"Updated dataset successfully. Comics loaded: {count}.");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Dataset update failed");
                status.LastOutcome = "Failed";
                status.LastError = ex.Message;
                SaveStatus(status);
                return (false, $"Update failed: {ex.Message}");
            }
            finally
            {
                TryDelete(zipTemp);
                TryDeleteDir(csvTempDir);
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    private DatasetUpdateStatus LoadStatus()
    {
        try
        {
            if (!File.Exists(StatusPath)) return new DatasetUpdateStatus();
            var json = File.ReadAllText(StatusPath);
            return JsonSerializer.Deserialize<DatasetUpdateStatus>(json) ?? new DatasetUpdateStatus();
        }
        catch
        {
            return new DatasetUpdateStatus();
        }
    }

    private void SaveStatus(DatasetUpdateStatus s)
    {
        var json = JsonSerializer.Serialize(s, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(StatusPath, json);
    }

    private static string Sha256File(string path)
    {
        using var sha = SHA256.Create();
        using var fs = File.OpenRead(path);
        var hash = sha.ComputeHash(fs);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static void TryDelete(string path)
    {
        try { if (File.Exists(path)) File.Delete(path); } catch { }
    }

    private static void TryDeleteDir(string path)
    {
        try { if (Directory.Exists(path)) Directory.Delete(path, recursive: true); } catch { }
    }
}