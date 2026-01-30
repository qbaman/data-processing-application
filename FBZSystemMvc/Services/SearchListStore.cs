using System.Text.Json;

namespace FBZSystemMvc.Services;

public class SearchListStore
{
    private const string Key = "SearchListIds";

    public List<string> GetIds(HttpContext http)
        => Get(http) ?? new List<string>();

    public void Add(HttpContext http, string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return;
        var ids = GetIds(http);
        if (!ids.Contains(id)) ids.Add(id);
        Save(http, ids);
    }

    public void Remove(HttpContext http, string id)
    {
        var ids = GetIds(http);
        ids.RemoveAll(x => x == id);
        Save(http, ids);
    }

    public void Clear(HttpContext http)
        => Save(http, new List<string>());

    private static List<string>? Get(HttpContext http)
    {
        var json = http.Session.GetString(Key);
        return json is null ? null : JsonSerializer.Deserialize<List<string>>(json);
    }

    private static void Save(HttpContext http, List<string> ids)
        => http.Session.SetString(Key, JsonSerializer.Serialize(ids));
}
