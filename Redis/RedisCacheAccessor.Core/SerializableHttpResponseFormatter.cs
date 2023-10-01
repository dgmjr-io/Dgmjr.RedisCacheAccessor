/*
 * SerializableHttpResponseFormatter.cs
 *
 *   Created: 2023-03-21-03:55:45
 *   Modified: 2023-03-21-03:55:45
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.RedisCacheAccessor.Core;

using System.Collections.Generic;
using System.Threading.Tasks;
using Dgmjr.RedisCacheAccessor.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;

public class SerializableHttpResponseFormatter : OutputFormatter
{
    private static readonly string[] _supportedMediaTypes = new[]
    {
        TextMediaTypeNames.Html,
        ApplicationMediaTypeNames.Json,
        TextMediaTypeNames.Plain,
        ApplicationMediaTypeNames.Xml,
        ApplicationMediaTypeNames.OctetStream,
        MultipartMediaTypeNames.FormData,
        ApplicationMediaTypeNames.FormUrlEncoded,
        VideoMediaTypeNames.Mp4,
        VideoMediaTypeNames.Mpeg,
        VideoMediaTypeNames.Ogg,
        ImageMediaTypeNames.Jpeg,
        ImageMediaTypeNames.Png,
        ImageMediaTypeNames.Gif
    };

    public SerializableHttpResponseFormatter()
    {
        this.SupportedMediaTypes.AddRange(_supportedMediaTypes);
    }

    public override IReadOnlyList<string>? GetSupportedContentTypes(
        string contentType,
        type objectType
    )
    {
        return CanWriteType(objectType) ? _supportedMediaTypes : Empty<string>();
    }

    protected override bool CanWriteType(System.Type? type) =>
        type == typeof(SerializableHttpResponse);

    public override bool CanWriteResult(OutputFormatterCanWriteContext context)
    {
        return context.Object is SerializableHttpResponse;
    }

    public override async Task WriteAsync(OutputFormatterWriteContext context)
    {
        if (context.Object is SerializableHttpResponse response)
        {
            // context.HttpContext.Response.StatusCode = response.StatusCode;
            WriteResponseHeaders(context);
            await WriteResponseBodyAsync(context);
        }
    }

    public override void WriteResponseHeaders(OutputFormatterWriteContext context)
    {
        if (context.Object is SerializableHttpResponse response)
        {
            foreach (var header in response.Headers)
            {
                context.HttpContext.Response.Headers.TryAdd(header.Key, header.Value);
            }
        }
        base.WriteResponseHeaders(context);
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
    {
        var response = context.Object as SerializableHttpResponse;
        if (response == null)
        {
            await context.HttpContext.Response.BodyWriter
                .AsStream()
                .WriteAsync(new ReadOnlyMemory<byte>(response.Content), default);
        }
    }
}
