using SpaceTradersAPI.Services;

namespace SpaceTradersAPI.Tests;

public class ServiceTest
{
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(25)]
    public async Task GetAllTest(int total)
    {
        var symbol = "test";

        var remaining = total;
        var fetcher = new Func<int, int, Task<DataGenerics<string[]>>>((innerPage, innerLimit) =>
        {
            var elementCount = Math.Min(remaining, Math.Min(innerLimit, total));

            var result = Task.FromResult(new DataGenerics<string[]>
            {
                Meta = new Meta
                {
                    Page = innerPage,
                    Limit = innerLimit,
                    Total = total
                },
                Data = Enumerable.Repeat("test", elementCount).ToArray()
            });
            remaining -= elementCount;
            return result;
        });

        var service = new SpaceTraders(null, null, null, null, symbol);

        var result = await service.GetAll(fetcher);
        Assert.Equal(total, result.Count);
    }
}