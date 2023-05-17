namespace SpaceTradersAPI.Services;

internal static class Helper
{
    public static async Task<List<T>> GetAll<T>(Func<int, int, Task<DataGenerics<T[]>>> fetcher)
    {
        const int limit = 20;

        var element = new List<T>();
        var resp = await fetcher(1, limit);
        element.AddRange(resp.Data);
        for (var i = resp.Meta.Page + 1;; i++)
        {
            if (resp.Meta.Page * limit >= resp.Meta.Total)
                break;

            resp = await fetcher(i, limit);
            element.AddRange(resp.Data);
        }

        return element;
    }
}