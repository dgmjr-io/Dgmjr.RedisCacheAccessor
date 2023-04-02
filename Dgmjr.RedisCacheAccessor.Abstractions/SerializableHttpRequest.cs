using System.Net.Http.Headers;
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

namespace Dgmjr.RedisCacheAccessor.Abstractions;

using System.Net.Http;
using System.Text;
using AnyOfTypes;
using AnyOfTypes.System.Text.Json;
public class SerializableHttpRequest
{
    public Uri RequestUri { get; set; }
    public System.Net.Http.Enums.HttpRequestMethod Method { get; set; }
    public string ContentType { get => Headers.TryGetValue(HttpRequestHeaderNames.ContentType, out var contentType) ? contentType : ApplicationMediaTypeNames.OctetStream; set => Headers[HttpRequestHeaderNames.ContentType] = value; }

    [JIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public byte[] BytesContent { get; set; }

    [JIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StringContent { get => BytesContent?.ToUTF8String(); set => BytesContent = value?.ToUTF8Bytes(); }
    private StringDictionary _headers = new(StringComparer.OrdinalIgnoreCase);
    public StringDictionary Headers { get => _headers ??= new StringDictionary(StringComparer.OrdinalIgnoreCase); set => _headers = new StringDictionary(value ?? new StringDictionary(), StringComparer.OrdinalIgnoreCase); }

    public SerializableHttpRequest(HttpRequestMessage message)
    {
        ContentType = message.Content.Headers.ContentType?.MediaType;
        BytesContent = message.Content.ReadAsByteArrayAsync().Result;
        Headers = message.Headers.ToDictionary(x => x.Key, x => Join(", ", x.Value));
        RequestUri = message.RequestUri;
        Method = HttpRequestMethod.TryParse(message.Method.Method, out var method) ? method.Value : HttpRequestMethod.Get.Value;
    }

    public SerializableHttpRequest()
    {

    }

    public static implicit operator HttpRequestMessage(SerializableHttpRequest request)
    {
        var message = new HttpRequestMessage(new HttpMethod(((HttpRequestMethod)request.Method).Name), request.RequestUri)
        {
            Content = request.ContentType.ToMediaType().IsText() ? new StringContent(request.StringContent) :
                new ByteArrayContent(request.BytesContent)
        };
        message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(request.ContentType);
        foreach (var header in request.Headers)
        {
            message.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return message;
    }

    public override string ToString()
    {
        return Serialize(this);
    }

    public static implicit operator SerializableHttpRequest(HttpRequestMessage message) => new(message);
}
