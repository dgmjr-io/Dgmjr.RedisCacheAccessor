/*
 * RedisCacheAccessor.cs
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
using System.Text;
using Dgmjr.RedisCacheAccessor.Abstractions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using static System.Net.HttpStatusCode;

namespace Dgmjr.RedisCacheAccessor.Core;

public class RedisCacheAccessor : IRedisCacheAccessor
{
    private readonly ICacheConnector _cacheConnector;

    private readonly ILogger log;

    public RedisCacheAccessor(ICacheConnector cacheConnector, ILoggerFactory loggerFactory)
    {
        _cacheConnector = cacheConnector;
        log = loggerFactory.CreateLogger("RedisCacheAccessor");
    }

    protected virtual IDatabase OpenDatabase(string cacheConnectionString)
    {
        return _cacheConnector.GetDbCache(cacheConnectionString);
    }

    /// <summary>
    /// Deletes the cached data. This is the asynchronous version of DeleteFromCache. The cache must be empty before this method is called
    /// </summary>
    /// </summary>
    /// </summary>
    /// </summary>
    /// </summary>
    /// </summary>
    /// <param name="cacheConnectionString">The connection string to the cache</param>
    /// <param name="cacheKey">The cache key to use for the cache</param>
    public Task<bool> DeleteFromCacheAsync(string cacheConnectionString, string cacheKey)
    {
        IDatabase dbCache = OpenDatabase(cacheConnectionString);
        return dbCache.KeyDeleteAsync(cacheKey);
    }

    /// <summary>Gets or sets rhe result of an HTTP request to the cache</summary?
    /// <param name="cacheConnectionString">The connection string to the redis cache</param>
    /// <param name="cacheKey">The key to the redis cache</param>
    /// <param name="requestMessage">The request message to send to the http endpoint</param>
    /// <param name="expiration">The expiration time for the cache key</param>
    /// >returns>the cache value for GET requests and the cache key for PUT <returns>
    public async Task<SerializableHttpResponse> GetFromCacheOrHttpAsync(string cacheConnectionString, string cacheKey, HttpRequestMessage requestMessage, TimeSpan? expiration = null)
    {
        IDatabase dbCache = OpenDatabase(cacheConnectionString);

        if (await dbCache.KeyExistsAsync(cacheKey))
        {
            var cachedValue = Deserialize<SerializableHttpResponse>(await dbCache.StringGetAsync(cacheKey));
            log.LogGet(cacheKey, cachedValue);
            return cachedValue;
        }
        else
        {
            var cachedValue = new SerializableHttpResponse(await new HttpClient().SendAsync(requestMessage), expiration);
            if (cachedValue != null)
            {
                await dbCache.StringSetAsync(cacheKey, Serialize(cachedValue), cachedValue.OriginalExpiration);
                log.LogPut(cacheKey, cachedValue, cachedValue.OriginalExpiration);
                return cachedValue;
            }
        }
        return new SerializableHttpResponse(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound), expiration);
    }

    /// <summary>
    /// Gets the keys from cache. This is the asynchronous version of GetKeysFromCache
    /// </summary>
    /// <param name="cacheConnectionString">The cache connection string.</param>
    /// <param name="pattern">The pattern to filter the keys. Default is "*"</param>
    public async Task<string[]> GetKeysFromCacheAsync(string cacheConnectionString, string pattern = "*")
    {
        IDatabase dbCache = OpenDatabase(cacheConnectionString);
        RedisResult rr = await dbCache.ExecuteAsync("keys", pattern);
        return ((RedisKey[]?)rr).Select(k => k.ToString()).ToArray();
    }


    /// <summary>
    /// Gets an instance of from the cache. If the cache doesn't exist a new instance is created
    /// </summary>
    /// <param name="cacheConnectionString">The cache connection string.</param>
    /// <param name="cacheKey">The cache key to use. This is used to cache the response and can be used to speed up subsequent requests</param>
    /// <param name="requestUrl">The request to fetch</param>
    /// <param name="expiration"></param>
    public async Task<SerializableHttpResponse> GetFromCacheOrHttpAsync(string cacheConnectionString, string cacheKey, string requestUrl, TimeSpan? expiration = null)
    {
        return await GetFromCacheOrHttpAsync(cacheConnectionString, cacheKey, new HttpRequestMessage(HttpMethod.Get, requestUrl), expiration);
    }

    /// <summary>
    /// Gets a value from the cache. If the value is not in the cache it will be set to the constant value
    /// </summary>
    /// <param name="cacheConnectionString">The cache connection string.</param>
    /// <param name="cacheKey">The key for the response. This is used to cache the response and can be used to optimize performance.</param>
    /// <param name="constantValue">The constant value to retrieve or store.</param>
    /// <param name="mimeType">The mime type of the value. Defaults to text / plain.</param>
    /// <param name="expiration">The expiration of the value. Defaults to 24 hours</param>
    public async Task<SerializableHttpResponse> GetFromCacheOrConstantValueAsync(string cacheConnectionString, string cacheKey, string constantValue, string mimeType = TextMediaTypeNames.Plain, TimeSpan? expiration = null)
    {
        var db = OpenDatabase(cacheConnectionString);
        if (db.KeyExists(cacheKey))
        {
            return Deserialize<SerializableHttpResponse>(await db.StringGetAsync(cacheKey))!;
        }
        else
        {
            var response = new SerializableHttpResponse(expiration)
            {
                Content = constantValue.ToUTF8Bytes(),
                ContentType = mimeType,
                StatusCode = (int)OK,
                ReasonPhrase = OK.ToString()
            };
            await db.StringSetAsync(cacheKey, Serialize(response), response.OriginalExpiration);
            return response;
        }
    }

    public Task<SerializableHttpResponse> GetFromCacheOrConstantValueAsync(string cacheConnectionString, string cacheKey, string constatntValue, TimeSpan? expiration = null)
        => GetFromCacheOrConstantValueAsync(cacheConnectionString, cacheKey, constatntValue, TextMediaTypeNames.Plain, expiration);

    /// <summary>
    /// Gets keys from cache. This is used to implement Cache. GetKeys () and Cache. GetCachedKeys (... )
    /// </summary>
    /// <param name="cacheConnectionString">Connection string to the cache</param>
    /// <param name="key"></param>
    public async Task<DateTimeOffset?> GetKeyExpiration(string cacheConnectionString, string key)
    {
        IDatabase dbCache = _cacheConnector.GetDbCache(cacheConnectionString);
        var ttl = await dbCache.KeyTimeToLiveAsync(key);
        if (ttl.HasValue)
            return DateTimeOffset.UtcNow + ttl.Value;
        else
            return default(DateTimeOffset?);
    }

    /// <summary>
    /// Converts a string to a data URI. This is used to create URIs that are compatible with Web. Net
    /// </summary>
    /// <param name="data">The data to convert.</param>
    /// <param name="mimeType">The MIME type of the data e. g</param>
    public static Uri ToDataUri(string data, string mimeType)
    {
        return ToDataUri(data.ToUTF8Bytes(), mimeType);
    }


    /// <summary>
    /// Converts a byte array to a data URI. This is used to create an object that can be used as a data URI and is suitable for sending to the server
    /// </summary>
    /// <param name="data">The data to convert.</param>
    /// <param name="mimeType">The MIME type of the data being converted e. g</param>
    protected static Uri ToDataUri(byte[] data, string mimeType)
    {
        ;
        var base64 = ToBase64String(data);
        return new Uri($"data:{mimeType};base64,{base64}");
    }
}
