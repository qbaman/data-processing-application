using System.Text.Json;

namespace FBZSystemMvc.Services;

public class RecentlyViewedStore
{
    private const string Key = "RecentlyViewedIds";
    private const int MaxItems = 5;

    public List<string> GetIds(HttpContext http)
        => Get(http) ?? new List<string>();

    public void Add(HttpContext http, string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return;
        var ids = GetIds(http);
        ids.RemoveAll(x => x == id);
        ids.Insert(0, id);
        if (ids.Count > MaxItems) ids = ids.Take(MaxItems).ToList();
        Save(http, ids);
    }

    private static List<string>? Get(HttpContext http)
    {
        var json = http.Session.GetString(Key);
        return json is null ? null : JsonSerializer.Deserialize<List<string>>(json);
    }

    private static void Save(HttpContext http, List<string> ids)
        => http.Session.SetString(Key, JsonSerializer.Serialize(ids));
}
