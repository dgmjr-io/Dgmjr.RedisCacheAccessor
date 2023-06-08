/*
 * DgmjrNamespace.cs
 *
 *   Created: 2023-01-10-10:37:17
 *   Modified: 2023-01-10-10:37:17
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

using StackExchange.Redis;

namespace Dgmjr.RedisCacheAccessor.Abstractions;

public interface ICacheConnector
{
    /// <summary>
    /// Return the redis cache database based on connection string
    /// </summary>
    /// <param name="cacheConnectionString">redis cache connection string</param>
    /// <returns></returns>
    IDatabase GetDbCache(string cacheConnectionString);
}
