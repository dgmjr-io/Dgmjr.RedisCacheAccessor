/* 
 * MimeTypesFilter.cs
 * 
 *   Created: 2023-03-18-06:41:19
 *   Modified: 2023-03-18-06:41:19
 * 
 *   Author: David G. Moore, Jr. <david@dgmjr.io>
 *   
 *   Copyright © 2022-2023 David G. Moore, Jr., All Rights Reserved
 *      License: MIT (https://opensource.org/licenses/MIT)
 */
namespace Dgmjr.GetOrSetFromRedisCache;

using Dgmjr.GetOrSetFromRedisCache.Controllers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

[RegexDto(@"^(?:([0-9]{0,4}\:)?(?:[0-9]{0,100}\:)?(?:[0-9]{0,1000})$")]
public partial struct TimeSpanDto
{

}

public class TimeSpanSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(TimeSpan) || context.Type == typeof(TimeSpan?))
        {
            schema.Type = "string";
            schema.Format = "time-span";
            schema.Description = "A time span in the format of <c>hh:mm:ss</c>";
            schema.Example = new OpenApiString("00:10:00");
            schema.Pattern = TimeSpanDto.RegexString;
        }
    }
}



// internal class RangeSchemaFilter : ISchemaFilter
// {
//     public void Apply(OpenApiSchema schema, SchemaFilterContext context)
//     {
//         if(context.MemberInfo == typeof(System.Range))
//         {
//             schema.
//         }
//     }
// }

// internal class MimeTypesFilter : ISchemaFilter
// {
//     public void Apply(OpenApiSchema schema, SchemaFilterContext context)
//     {
//         if(context.MemberInfo.Name == "Get" || context.MemberInfo.Name == "Put" || context.MemberInfo.Name == "Post")
//             schema.Enum = new System.Collections.Generic.List<IOpenApiAny>()
//             {
//                 new OpenApiString(ApplicationMediaTypeNames.FormUrlEncoded),
//                 new OpenApiString(ApplicationMediaTypeNames.Json),
//                 new OpenApiString(ApplicationMediaTypeNames.JsonPatch),
//                 new OpenApiString(ApplicationMediaTypeNames.OctetStream),
//                 new OpenApiString(ApplicationMediaTypeNames.Xml),
//                 new OpenApiString(ImageMediaTypeNames.Gif),
//                 new OpenApiString(ImageMediaTypeNames.Jpeg),
//                 new OpenApiString(ImageMediaTypeNames.Png),
//                 new OpenApiString(MultipartMediaTypeNames.FormData),
//                 new OpenApiString(TextMediaTypeNames.Css),
//                 new OpenApiString(TextMediaTypeNames.Csv),
//                 new OpenApiString(TextMediaTypeNames.Html),
//                 // new OpenApiString(TextMediaTypeNames.Markdown),
//                 new OpenApiString(TextMediaTypeNames.Plain),
//                 new OpenApiString(VideoMediaTypeNames.Mp4),
//                 new OpenApiString(VideoMediaTypeNames.Mpeg),
//                 new OpenApiString(VideoMediaTypeNames.Ogg)
//             };
//     }
// }
