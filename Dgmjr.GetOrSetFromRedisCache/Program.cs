using Azure.Identity;
using Dgmjr.GetOrSetFromRedisCache;
using Dgmjr.RedisCacheAccessor.Core;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using Constants = Dgmjr.GetOrSetFromRedisCache.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHealthChecks();
// builder.Configuration.AddUserSecrets<Program>();
builder.AddTheWorks(new StartupParametersBuilder()
    .WithThisAssemblyProject(typeof(ThisAssembly.Project))
    .WithAddConsoleLogger(true)
    .WithoutSearchEntireAppDomainForAutoMapperAndMediatRTypes()
    .WithoutAddIdentity()
    .WithoutAddDefaultIdentityUI()
    .WithoutAddRazorPages()
    .WithoutAddAutoMapper()
    .WithoutAddHashids()
    .WithoutAddJsonPatch()
    // .WithAddHealthChecks(
    .WithoutAddApiAuthentication()
    .WithoutAddMediatR()
    .WithoutAddSendPulseApi()
    .WithAddSwagger(true)
    .WithAzureAppConfiguration(config => config.Connect(builder.Configuration[Constants.AZURE_APPCONFIGURATION_CONNECTION_STRING]),
        config => config.SetCredential(new DefaultAzureCredential()))
    .Build());

// {
//     AddConsoleLogger = true,
//     SearchEntireAppDomainForAutoMapperAndMediatRTypes = false,
//     AddIdentity = false,
//     AddDefaultIdentityUI = false,
//     AddRazorPages = false,
//     AddAzureAppConfig = true,
//     AddAutoMapper = false,
//     AddHashids = false,
//     AddJsonPatch = false,
//     AddHealthChecks = true,
//     AddApiAuthentication = false,
//     AddMediatR = false,
//     AddSendPulseApi = false,
//     AddSwagger = true,
// }.AddAzureAppConfiguration(_ => {}, _ => {}));
builder.Services.Configure<RedisCacheConfiguration>(config => config.ConnectionString = builder.Configuration[Constants.REDIS_CACHE_CONNECTION_STRING]);
builder.Services.Configure<TelemetryConfiguration>(config =>
{
    config.InstrumentationKey = builder.Configuration[Constants.APPINSIGHTS_INSTRUMENTATIONKEY];
    config.ConnectionString = builder.Configuration[Constants.APPLICATIONINSIGHTS_CONNECTION_STRING];
});
builder.Logging.AddApplicationInsights();//builder.Configuration.GetValue<TelemetryConfiguration>("ApplicationInsights:TelemetryConfiguration"), _ => {});
builder.Services.AddControllers().AddMvcOptions(mvc =>
{
    mvc.OutputFormatters.Insert(0, new SerializableHttpResponseFormatter());
});
builder.Services.ConfigureSwaggerGen(c => c.SchemaFilter<TimeSpanSchemaFilter>());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.AllowInputFormatterExceptionMessages = true;
    x.JsonSerializerOptions.AllowTrailingCommas = true;
    x.JsonSerializerOptions.DefaultIgnoreCondition = JIgnoreCond.WhenWritingNull;
    x.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    x.JsonSerializerOptions.IgnoreReadOnlyFields = false;
    x.JsonSerializerOptions.IgnoreReadOnlyProperties = false;
    x.JsonSerializerOptions.IncludeFields = true;
    x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    x.JsonSerializerOptions.NumberHandling =
        JNumberHandling.AllowReadingFromString
        | JNumberHandling.AllowNamedFloatingPointLiterals;
    x.JsonSerializerOptions.ReadCommentHandling = JCommentHandling.Skip;
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    x.JsonSerializerOptions.UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement;
    x.JsonSerializerOptions.WriteIndented = true;
    x.JsonSerializerOptions.Converters.Add(new JStringEnumConverter());
    // x.JsonSerializerOptions.Converters.Add(new AnyOfTypes.System.Text.Json.AnyOfJsonConverter());
});
// builder.Services.AddSwaggerGen(
//         c => 
//         {
//             c.CustomSchemaIds(x => x.FullName);
//             c.SwaggerDoc("v1", new OpenApiInfo()
//             {
//                 Title = "Get or Set from The Backroom's Redis Cache",
//                 Version = "v1",
//                 Contact = new OpenApiContact() { Name = "Justin Chase", Email = "justin@thebackroom.bot" },
//                 License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") },
//                 Description =
//             """"
//             This service is used to cache HTTP responses.  It can be used in several ways:

//             # GET

//             nUsed to cache <c>GET</c> requests from an HTTP service.  Cannnot be used to send any headers or <c>post</c> any information.  Can be used for simple <c>get</b> requests only.

//             # PUT or POST

//             These methods can be used to call <c>PUT</c> or <c>POST</c> requests to an HTTP service.  They receive a payload of the type <c>HttpRequestMessage</c>, which contains all the information required to <c>put</c> or <c>post</c> to the requested HTTP endpoint.  

//             *Note if sending header information along with a <c>get</c> request, these can be used for that purpose too.  Simply set their HTTP methods to <c>get</c>*.

//             # DELETE

//             Use this method to delete a cached item from the Redis cache.  Simply provide the <c>cacheKey</c> as a query parameter.
//             """"
//             });
//             // c.SchemaFilter<RangeSchemaFilter>();
//         }
//     );
// builder.Logging.AddConsole();
// builder.Logging.SetMinimumLevel(LogLevel.Debug);


builder.Services.AddSingleton<Dgmjr.RedisCacheAccessor.Abstractions.IRedisCacheAccessor, Dgmjr.RedisCacheAccessor.Core.RedisCacheAccessor>();
builder.Services.AddSingleton<Dgmjr.RedisCacheAccessor.Abstractions.ICacheConnector, Dgmjr.RedisCacheAccessor.Core.CacheConnector>();

var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(c =>
//     {
//         c.SwaggerEndpoint("/swagger/v1/swagger.json", "Get or Set from The Backroom's Redis Cache v1");
//         c.DisplayOperationId();
//         c.DefaultModelRendering(ModelRendering.Model);
//         c.DocumentTitle = "Get or Set from The Backroom's Redis Cache";
//         c.EnableDeepLinking();
//         c.EnableFilter();
//         c.EnableTryItOutByDefault();
//         c.DisplayRequestDuration();
//         c.ShowCommonExtensions();
//         c.ShowExtensions();
//         c.SupportedSubmitMethods(new[] { SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put, SubmitMethod.Delete, SubmitMethod.Head, SubmitMethod.Options, SubmitMethod.Patch, SubmitMethod.Trace });
//     });
// }

app.UseTheWorks(typeof(ThisAssembly.Project));

// app.UseHttpsRedirection();

// app.UseAuthorization();

// app.MapControllers();

app.Run();
