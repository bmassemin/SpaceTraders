﻿using Microsoft.Extensions.Logging;
using SpaceTradersAPI.Repositories;
using SpaceTradersAPI.Services;

namespace SpaceTradersAPI.Factories;

public static class ServiceFactory
{
    public static SpaceTraders CreateService(ILoggerFactory accountLoggerFactory, string symbol, string token)
    {
        var client = new Client(HttpClientFactory.CreateClient(accountLoggerFactory.CreateLogger(symbol)));
        client.SetToken(token);

        var systemService = new SystemService(client);
        var systemCache = new SystemCacheService(systemService);
        var shipLockRepo = new MemoryShipLockRepository();
        return new SpaceTraders(accountLoggerFactory.CreateLogger(symbol), client, systemCache, shipLockRepo, symbol);
    }
}