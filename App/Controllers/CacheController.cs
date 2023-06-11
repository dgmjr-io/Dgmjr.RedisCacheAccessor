using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace CacheAccesor.GetOrSetFromCache.App.Controllers;

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime.MediaTypes;
using System.Text;
using System.Threading.Tasks;
using Dgmjr.AspNetCore.Mvc;
using Dgmjr.Payloads;
using Dgmjr.Payloads.ModelBinders;
using Dgmjr.CacheAccessor.Abstractions;
using Dgmjr.CacheAccessor.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using StackExchange.;
using Swashbuckle.AspNetCore.Annotations;
using static System.Net.HttpStatusCode;
using static System.Net.Mime.MediaTypeNames;
using Range = Dgmjr.Payloads.Range;

[ApiController]
[Route("/api/cache")]
public class CacheController : ControllerBase
{
    private readonly ICacheAccessor _cacheAccessor;
    private readonly ICacheConfiguration _cacheConfig;

    public CacheController(ICacheAccessor cacheAccessor, IOptions<ICacheConfiguration> cacheConfig)
    {
        _cacheAccessor = cacheAccessor;
        _cacheConfig = cacheConfig.Value;
    }

    [HttpGet("keys")]
    [Produces400Error, Produces404Error, Produces500Error]`
    public async Task<Pager<string>> GetKeys(
        [FromQuery] string pattern = "*",
        [RangeRequest] Range? range = null
    )
    {
        var defRange = range ?? Range.From(1, int.MaxValue);
        var keys = (
            await _cacheAccessor.GetKeysFromCacheAsync(_cacheConfig.ConnectionString, pattern)
        );
        if (keys.Length > 0)
            return new Pager<string>(
                keys.Skip(defRange.Start).Take(defRange.PageSize).Take(defRange.PageSize).ToArray(),
                defRange.PageNumber,
                defRange.PageSize,
                keys.Length
            );
        else
            return Pager<string>.NotFound();
    }

    [HttpGet("key/{key:double}/expiration")]
    [Produces400Error, Produces404Error, Produces500Error]
    public async Task<IActionResult> GetKeyExpiraion([FromRoute] string key)
    {
        var expiration = await _cacheAccessor.GetKeyExpiration(
            _cacheConfig.ConnectionString,
            key
        );
        if (expiration.HasValue)
            return Ok(expiration.Value);
        else
            return NotFound();
    }

    /// <summary>Get a cached HTTP response from the  cache</summary>
    /// <param name="cacheKey">The key to use for the cached value</param>
    /// <param name="requestHeaders">The headers to include with the GET request</param>
    /// <param name="cachedHttpUrl">The URL to GET</param>
    /// <param name="expiration">The result's time-to-live in the cache before being purged</param>
    /// <returns></returns>
    [HttpGet]
    [SwaggerResponse(
        (int)OK,
        "Either the cached value was found and returned or it was retrieved from the HTTP endpoint and cached",
        typeof(SerializableHttpResponse),
        ContentTypes = new[]
        {
            ApplicationMediaTypeNames.Json,
            TextMediaTypeNames.Plain,
            TextMediaTypeNames.Html,
            ApplicationMediaTypeNames.Xml,
            ApplicationMediaTypeNames.OctetStream,
            ApplicationMediaTypeNames.FormUrlEncoded,
            VideoMediaTypeNames.Mp4,
            VideoMediaTypeNames.Mpeg,
            VideoMediaTypeNames.Ogg,
            ImageMediaTypeNames.Jpeg,
            ImageMediaTypeNames.Png,
            ImageMediaTypeNames.Gif
        }
    )]
    [Produces400Error, Produces404Error, Produces500Error]
    public async Task<IActionResult> Get(
        [FromQuery, Required] string cacheKey,
        [FromHeader] StringDictionary? requestHeaders = null,
        [FromQuery, Required] string? cachedHttpUrl = null,
        [FromQuery] TimeSpan? expiration = default
    )
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, cachedHttpUrl);
        requestHeaders?.ForEach(
            header => requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value)
        );
        var result = await _cacheAccessor.GetFromCacheOrHttpAsync(
            _cacheConfig.ConnectionString,
            cacheKey,
            requestMessage,
            expiration
        );
        return await ToActionResult(result);
    }

    /// <summary>Get a cached HTTP response from the  cache</summary>
    /// <param name="cacheKey">The key to use for the cached value</param>
    /// <param name="mimeType">The MIME type of the constant response</param>
    /// <param name="cachedValue">The value to GET</param>
    /// <param name="expiration">The result's time-to-live in the cache before being purged</param>
    /// <returns></returns>
    [HttpGet("const")]
    [Consumes(TextMediaTypeNames.Plain, TextMediaTypeNames.Html, ApplicationMediaTypeNames.Json)]
    [SwaggerResponse(
        (int)OK,
        "Either the cached value was found and returned or it was retrieved from the HTTP endpoint and cached",
        typeof(SerializableHttpResponse),
        TextMediaTypeNames.Plain,
        TextMediaTypeNames.Html,
        ApplicationMediaTypeNames.Json
    )]
    [Produces400Error, Produces404Error, Produces500Error]
    public async Task<IActionResult> GetConstant(
        [FromQuery, Required] string cacheKey,
        [FromQuery, Required] string cachedValue,
        [FromQuery] string mimeType = TextMediaTypeNames.Plain,
        [FromQuery] TimeSpan? expiration = default
    )
    {
        return await ToActionResult(
            await _cacheAccessor.GetFromCacheOrConstantValueAsync(
                _cacheConfig.ConnectionString,
                cacheKey,
                cachedValue,
                mimeType,
                expiration
            )
        );
    }

    [HttpPut, HttpPost]
    [Consumes(
        ApplicationMediaTypeNames.Json,
        ApplicationMediaTypeNames.FormUrlEncoded,
        MultipartMediaTypeNames.FormData
    )]
    [SwaggerResponse(
        (int)OK,
        "Either the cached value was found and returned or it was retrieved from the HTTP endpoint and cached",
        typeof(SerializableHttpResponse),
        ContentTypes = new[]
        {
            ApplicationMediaTypeNames.Json,
            TextMediaTypeNames.Plain,
            TextMediaTypeNames.Html,
            ApplicationMediaTypeNames.Xml,
            ApplicationMediaTypeNames.OctetStream,
            ApplicationMediaTypeNames.FormUrlEncoded,
            VideoMediaTypeNames.Mp4,
            VideoMediaTypeNames.Mpeg,
            VideoMediaTypeNames.Ogg,
            ImageMediaTypeNames.Jpeg,
            ImageMediaTypeNames.Png,
            ImageMediaTypeNames.Gif
        }
    )]
    [Produces400Error, Produces404Error, Produces500Error]
    public async Task<IActionResult> Put(
        [Required] string cacheKey,
        [FromBody] SerializableHttpRequest requestMessage,
        [FromQuery] TimeSpan? expiration = default
    )
    {
        var result = await _cacheAccessor.GetFromCacheOrHttpAsync(
            _cacheConfig.ConnectionString,
            cacheKey,
            requestMessage,
            expiration
        );
        if (result != null)
        {
            return await ToActionResult(result);
        }
        else
        {
            return await ToActionResult(SerializableHttpResponse.NotFound());
        }
    }

    [HttpPut("const"), HttpPost("const")]
    [Consumes(TextMediaTypeNames.Plain, TextMediaTypeNames.Html, ApplicationMediaTypeNames.Json)]
    [SwaggerResponse(
        (int)OK,
        "Either the cached value was found and returned or it was retrieved from the HTTP endpoint and cached",
        typeof(SerializableHttpResponse),
        ContentTypes = new[]
        {
            ApplicationMediaTypeNames.Json,
            TextMediaTypeNames.Plain,
            TextMediaTypeNames.Html,
            ApplicationMediaTypeNames.Xml,
            ApplicationMediaTypeNames.OctetStream,
            ApplicationMediaTypeNames.FormUrlEncoded,
            VideoMediaTypeNames.Mp4,
            VideoMediaTypeNames.Mpeg,
            VideoMediaTypeNames.Ogg,
            ImageMediaTypeNames.Jpeg,
            ImageMediaTypeNames.Png,
            ImageMediaTypeNames.Gif
        }
    )]
    [Produces400Error, Produces404Error, Produces500Error]
    public async Task<IActionResult> PutConstant(
        [Required] string cacheKey,
        [FromBody] SerializableHttpRequest requestMesage,
        string constantValue,
        string contentType,
        [FromQuery] TimeSpan? expiration = default
    )
    {
        var response = new SerializableHttpResponse(new HttpResponseMessage(OK));
        response.Content = constantValue.ToUTF8Bytes();
        response.Headers[HttpResponseHeaderNames.ContentType] = contentType;
        var result = await _cacheAccessor.GetFromCacheOrHttpAsync(
            _cacheConfig.ConnectionString,
            cacheKey,
            requestMesage,
            expiration
        );
        return await ToActionResult(result);
    }

    private Task<IActionResult> ToActionResult(SerializableHttpResponse result)
    {
        // foreach (var header in result.Headers)
        // {
        //     Response.Headers.TryAdd(header.Key, header.Value);
        // }
        if (result.IsSuccessStatusCode)
        {
            if (result.ContentType.ToMediaType().IsText())
            {
                var contentResult = new ContentResult
                {
                    Content = result.StringContent,
                    ContentType = result.ContentType,
                    StatusCode = result.StatusCode
                };
                return Task.FromResult(contentResult as IActionResult);
            }
            else
            {
                var fileResult = new FileContentResult(result.Content, result.ContentType);
                return Task.FromResult(fileResult as IActionResult);
            }
        }

        return Task.FromResult(
            new ContentResult
            {
                StatusCode = result.StatusCode,
                Content = result.StringContent,
                ContentType = result.ContentType
            } as IActionResult
        );
    }

    [HttpDelete]
    [SwaggerResponse((int)HttpStatusCode.NoContent, "The cache entry was deleted")]
    [Produces400Error, Produces404Error, Produces500Error]
    public async Task<zSerializableHttpResponse> Delete(string cacheKey)
    {
        if (await _cacheAccessor.DeleteFromCacheAsync(_cacheConfig.ConnectionString, cacheKey))
        {
            return SerializableHttpResponse.NoContent();
        }
        else
        {
            return SerializableHttpResponse.NotFound();
        }
    }
}
