/*
 * LoggingExtensions.cs
 *
 *   Created: 2023-03-18-01:32:04
 *   Modified: 2023-03-18-01:32:04
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace CacheAccesor;

using System;
using System.Net.Http;
using Dgmjr.RedisCacheAccessor.Abstractions;
using Microsoft.Extensions.Logging;

public static partial class LoggingExtensions
{
    [LoggerMessage(
        1,
        LogLevel.Information,
        "PUT cacheKey={cacheKey} cacheValue={cacheValue} expiration={expiration}",
        HttpRequestMethodNames.Put
    )]
    public static partial void LogPut(
        this ILogger logger,
        string cacheKey,
        object cacheValue,
        TimeSpan expiration
    );

    [LoggerMessage(
        2,
        LogLevel.Information,
        "GET cacheKey={cacheKey} cacheValue={cacheValue}",
        HttpRequestMethodNames.Get
    )]
    public static partial void LogGet(this ILogger logger, string cacheKey, object cacheValue);

    [LoggerMessage(
        3,
        LogLevel.Information,
        "DELETE cacheKey={cacheKey}",
        HttpRequestMethodNames.Delete
    )]
    public static partial void LogDelete(this ILogger logger, string cacheKey);

    [LoggerMessage(4, LogLevel.Information, "FIND key like \"{pattern}\", range: {range}", "FIND")]
    public static partial void LogFindKeys(
        this ILogger logger,
        string pattern,
        Dgmjr.Payloads.Range range
    );

    [LoggerMessage(100, LogLevel.Information, "Sending {request}...", "SENDING_REQUEST")]
    public static partial void LogSendingRequest(
        this ILogger logger,
        SerializableHttpRequest request
    );
}
