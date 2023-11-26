/*
 * ICacheConfiguration.cs
 *
 *   Created: 2023-06-11-06:00:56
 *   Modified: 2023-06-11-06:00:56
 *
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *
 *   Copyright © 2022 - 2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */

namespace CacheAccesor.Abstractions;

public interface ICacheConfiguration
{
    Cachelocation CacheLocation { get; set; }
    string ConnectionString { get; set; }
}
