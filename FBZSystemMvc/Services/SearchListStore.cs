using FBZSystemMvc.Services;

namespace FBZSystemMvc.Services;

public class SearchListStore
{
    private const string Key = "SearchListIds";

    public List<int> GetIds(HttpContext ctx)
        => ctx.Session.GetJson<List<int>>(Key) ?? new List<int>();

    public void SaveIds(HttpContext ctx, List<int> ids)
        => ctx.Session.SetJson(Key, ids);
}
