/*
 * Constants.cs
 *
 *   Created: 2023-05-05-12:26:18
 *   Modified: 2023-05-05-12:26:19
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022 - 2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace CacheAccesor;

public static partial class Constants
{
    public const string AZURE_APPCONFIGURATION_CONNECTION_STRING = nameof(
        AZURE_APPCONFIGURATION_CONNECTION_STRING
    );
    public const string AZURE_BLOB_CONNECTION_STRING = nameof(AZURE_BLOB_CONNECTION_STRING);
    public const string AZURE_FILESCONNECTION_STRING = nameof(AZURE_FILESCONNECTION_STRING);
    public const string AZURE_APPCONFIGURATION_ENDPOINT = nameof(AZURE_APPCONFIGURATION_ENDPOINT);
    public const string AZURE_KEYVAULT_RESOURCEENDPOINT = nameof(AZURE_KEYVAULT_RESOURCEENDPOINT);
    public const string APPLICATIONINSIGHTS_CONNECTION_STRING = nameof(
        APPLICATIONINSIGHTS_CONNECTION_STRING
    );
    public const string APPINSIGHTS_INSTRUMENTATIONKEY = nameof(APPINSIGHTS_INSTRUMENTATIONKEY);
    public const string REDIS_CACHE_CONNECTION_STRING = nameof(REDIS_CACHE_CONNECTION_STRING);
}
