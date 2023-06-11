/* 
 * IKeyExpirationTuple.cs
 * 
 *   Created: 2023-06-11-06:05:34
 *   Modified: 2023-06-11-06:05:35
 * 
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *   
 *   Copyright © 2022 - 2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace CacheAccesor.Abstractions;

public interface IKeyExpirationTuple
{
    string Key { readonly get; set; }
    TimeSpan OriginalTimeToLive { readonly get; set; }
    TimeSpan CurrentTimeToLive { readonly get; set; }
    DateTimeOffset Expiration { readonly get; set; }
}
