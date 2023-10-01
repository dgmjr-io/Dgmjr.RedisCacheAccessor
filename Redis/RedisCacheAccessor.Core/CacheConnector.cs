/*
 * CachConnector.cs
 *
 *   Created: 2023-01-10-10:37:17
 *   Modified: 2023-01-10-10:37:17
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

using System.Collections.Generic;
using Dgmjr.RedisCacheAccessor.Abstractions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Dgmjr.RedisCacheAccessor.Core;

public class CacheConnector : Dgmjr.RedisCacheAccessor.Abstractions.ICacheConnector
{
    /// <summary>
    /// Static dictionary to hold all redis connections.  Dictionary key is the
    /// redis cache connection string
    /// </summary>
    private static Dictionary<string, ConnectionMultiplexer> redisConnections;

    /// <summary>
    /// Returns the instance of the redis cache based on the cache
    /// connection string passed in.
    /// </summary>
    /// <param name="cacheConnectionString"></param>
    /// <returns>Instance of the redis cache</returns>
    public IDatabase GetDbCache(string cacheConnectionString)
    {
        //instantiate dictionary if null
        redisConnections ??= new Dictionary<string, ConnectionMultiplexer>();

        //Retrieve the connection from the dictionary.  If it doesn't exist
        //add the connection to the dictionary
        if (!redisConnections.TryGetValue(cacheConnectionString, out var connection))
        {
            connection = ConnectionMultiplexer.Connect(cacheConnectionString);
            redisConnections.Add(cacheConnectionString, connection);
        }

        IDatabase dbCache = connection.GetDatabase();
        return dbCache;
    }
}
