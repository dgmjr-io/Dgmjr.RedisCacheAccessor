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

using System.Net.Http;

namespace Dgmjr.RedisCacheAccessor.Abstractions;

public interface IRedisCacheAccessor
{
    Task<bool> DeleteFromCacheAsync(string cacheConnectionString, string cacheKey);
    Task<SerializableHttpResponse> GetFromCacheOrConstantValueAsync(string cacheConnectionString, string cacheKey, string constantValue, string mimeType = "text/plain", TimeSpan? expiration = null);
    Task<SerializableHttpResponse> GetFromCacheOrConstantValueAsync(string cacheConnectionString, string cacheKey, string constatntValue, TimeSpan? expiration = null);
    Task<SerializableHttpResponse> GetFromCacheOrHttpAsync(string cacheConnectionString, string cacheKey, HttpRequestMessage requestMessage, TimeSpan? expiration = null);
    Task<SerializableHttpResponse> GetFromCacheOrHttpAsync(string cacheConnectionString, string cacheKey, string requestUrl, TimeSpan? expiration = null);
    Task<DateTimeOffset?> GetKeyExpiration(string cacheConnectionString, string key);
    Task<string[]> GetKeysFromCacheAsync(string cacheConnectionString, string pattern = "*");
}
