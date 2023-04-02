/* 
 * RedisKeyExpirationTuple.cs
 * 
 *   Created: 2023-03-23-01:10:44
 *   Modified: 2023-03-23-01:10:45
 * 
 *   Author: Justin Chase <justin@justinwritescode.com>
 *   
 *   Copyright © 2022-2023 Justin Chase, All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace Dgmjr.RedisCacheAccessor.Abstractions;

public struct RedisKeyExpirationTuple
{
    public string Key { get; set; }
    public TimeSpan OriginalTimeToLive { get; set; }
    public TimeSpan CurrentTimeToLive { get; set; }
    public DateTimeOffset Expiration { get; set; }
}
