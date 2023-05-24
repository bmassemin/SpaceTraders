using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpaceTradersAPI.Tests;

public class ModelTest
{
    [Fact]
    public void TestNavigationPayload()
    {
        var defaultSerializationOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };
        var payload =
            "{\"data\":{\"nav\":{\"systemSymbol\":\"X1-VS75\",\"waypointSymbol\":\"X1-VS75-21813Z\",\"route\":{\"departure\":{\"symbol\":\"X1-VS75-70500X\",\"type\":\"PLANET\",\"systemSymbol\":\"X1-VS75\",\"x\":5,\"y\":9},\"destination\":{\"symbol\":\"X1-VS75-21813Z\",\"type\":\"MOON\",\"systemSymbol\":\"X1-VS75\",\"x\":5,\"y\":9},\"arrival\":\"2023-05-24T08:03:20.678Z\",\"departureTime\":\"2023-05-24T08:03:05.678Z\"},\"status\":\"IN_TRANSIT\",\"flightMode\":\"CRUISE\"},\"fuel\":{\"current\":1119,\"capacity\":1200,\"consumed\":{\"amount\":1,\"timestamp\":\"2023-05-24T08:03:05.690Z\"}}}}";

        var result = JsonSerializer.Deserialize<DataGenerics<DataNavigate>>(payload, defaultSerializationOptions);
    }
}