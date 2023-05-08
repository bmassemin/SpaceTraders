using Refit;
using SpaceTraders.API;

namespace SpaceTraders;

internal interface IClient
{
    [Post("/v2/register")]
    Task<DataResponse> RegisterNewAgentAsync([Body] RegisterNewAgentRequest request);

    [Get("/v2/my/agent")]
    Task<DataGenerics<Agent>> AgentDetailsAsync();
}