/*
 * HttpResponse.cs
 * 
 *   Created: 2023-03-18-09:17:41
 *   Modified: 2023-03-18-09:17:41
 * 
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *   
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Net.Mime.MediaTypes;
using System.Text;
using AnyOfTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;
using static System.Net.HttpStatusCode;
using MediaTypeNames = System.Net.Mime.MediaTypes.MediaTypeNames;

namespace Dgmjr.RedisCacheAccessor.Abstractions;

public class SerializableHttpResponse
{
    public const int DefaultExpirationSeconds = 86400;
    public static TimeSpan DefaultExpiration => TimeSpan.FromSeconds(DefaultExpirationSeconds);
    public const string UnixEpoch = "1970-01-01T00:00:00.0000000Z";
    public static DateTimeOffset UnixEpochDateTime => DateTimeOffset.Parse(UnixEpoch);

    public const string XOriginalExpiration = "X-Original-Expiration";
    public const string XCreatedDateTime = "X-Created-DateTime";

    public string ContentType { get => Headers.TryGetValue(HReqH.ContentType, out var contentType) ? contentType : ApplicationMediaTypeNames.OctetStream; set => Headers[HttpRequestHeaderNames.ContentType] = value; }
    [JIgnore]
    public IMediaType MediaType { get => ContentType.ToMediaType(); set => ContentType = value.Name; }
    public byte[] Content { get; set; }
    [JIgnore]
    public string StringContent { get => Content.ToUTF8String(); set => Content = value.ToUTF8Bytes(); }
    public SerializableHttpResponse(int statusCode)
    {
        this.StatusCode = statusCode;

    }
    public int StatusCode { get; set; }

    private StringDictionary _headers;
    public StringDictionary Headers { get => _headers; set => _headers = new(value, StringComparer.OrdinalIgnoreCase); }
    public string? ReasonPhrase { get; set; }
    public bool IsSuccessStatusCode => StatusCode >= 200 && StatusCode <= 299;
    public DateTimeOffset CreatedDateTime { get => DateTimeOffset.Parse(Headers.TryGetValue(XCreatedDateTime, out var createdDateTime) ? createdDateTime : UnixEpoch); set => Headers[XCreatedDateTime] = value.ToString(); }
    public TimeSpan OriginalExpiration { get => TimeSpan.FromSeconds(double.Parse(Headers.TryGetValue(XOriginalExpiration, out var originalExpiration) ? originalExpiration : "0")); set => Headers[XOriginalExpiration] = value.TotalSeconds.ToString(); }
    public DateTimeOffset ExpirationDateTime { get => CreatedDateTime + OriginalExpiration; set => OriginalExpiration = value - CreatedDateTime; }
    public TimeSpan TimeToLive { get => TimeSpan.FromTicks(Math.Abs((DateTimeOffset.Now - (CreatedDateTime + OriginalExpiration)).Ticks)); set { } }

    public SerializableHttpResponse(HttpResponseMessage message, TimeSpan? expiration = default) : this(expiration)
    {
        expiration ??= DefaultExpiration;
        ContentType = message.Content.Headers.ContentType?.MediaType ??
            (message.Headers.TryGetValues(HttpResponseHeaderNames.ContentType, out var values) ? Join(", ", values) :
            message.Content.Headers.TryGetValues(HttpResponseHeaderNames.ContentType, out values) ? Join(", ", values) : ApplicationMediaTypeNames.OctetStream);
        Content = message.Content.ReadAsByteArrayAsync().Result;
        StatusCode = (int)message.StatusCode;
        _headers = _headers.Concat(message.Headers.ToDictionary(x => x.Key, x => x.Value.First())).ToDictionary(x => x.Key, x => x.Value);
        ReasonPhrase = message.ReasonPhrase;
    }

    public SerializableHttpResponse(TimeSpan? expiration = default)
    {
        _headers = new StringDictionary(StringComparer.OrdinalIgnoreCase);
        OriginalExpiration = expiration ?? DefaultExpiration;
        CreatedDateTime = DateTimeOffset.UtcNow;
    }

    public SerializableHttpResponse() : this(default(TimeSpan?)) { }

    public static SerializableHttpResponse NoContent()
        => new(default(TimeSpan?))
        {
            StatusCode = (int)HttpStatusCode.NoContent,
            Content = Empty<byte>(),
            Headers = new()
            {
                [HttpResponseHeaderNames.ContentType] = TextMediaTypeNames.Plain,
                [HttpResponseHeaderNames.ContentLength] = "0"
            }
        };
    public static SerializableHttpResponse BadRequest()
        => new(default(TimeSpan?))
        {
            StatusCode = (int)HttpStatusCode.BadRequest,
            StringContent = HttpStatusCode.BadRequest.ToString(),
            Headers = new()
            {
                [HttpResponseHeaderNames.ContentType] = TextMediaTypeNames.Plain,
            }
        };
    public static SerializableHttpResponse NotFound()
        => new(default(TimeSpan?))
        {
            StatusCode = (int)HttpStatusCode.NotFound,
            StringContent = HttpStatusCode.NotFound.ToString(),
            Headers = new()
            {
                [HttpResponseHeaderNames.ContentType] = TextMediaTypeNames.Plain,
            }
        };

    public static implicit operator HttpResponseMessage(SerializableHttpResponse response)
    {
        var message = new HttpResponseMessage((HttpStatusCode)response.StatusCode)
        {
            Content = new ByteArrayContent(response.Content),
            ReasonPhrase = response.ReasonPhrase,
        };
        message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(response.ContentType);
        foreach (var header in response.Headers)
        {
            message.Headers.Add(header.Key, header.Value);
        }

        return message;
    }
}
