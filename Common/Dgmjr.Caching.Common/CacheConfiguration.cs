/*
 * RedisCacheConfiguration.cs
 *
 *   Created: 2023-03-21-01:30:44
 *   Modified: 2023-03-21-01:30:44
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

using Dgmjr.Cachingc;

namespace Dgmjr.Caching;

public class CacheConfiguration
{
    public Cachelocation CacheLocation { get; set; } = CacheLocation.Memory;
    public string? ConnectionString { get; set; }
}
