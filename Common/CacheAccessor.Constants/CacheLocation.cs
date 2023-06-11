/*
 * Cachelocation.cs
 *
 *   Created: 2023-05-05-06:13:50
 *   Modified: 2023-05-05-06:13:51
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022 - 2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace CacheAccesor;

public static partial class Constants
{
    public enum CacheLocation
    {
        Memory,
        Redis,
        AzureBlobs,
        AzureFiles
    }
}
