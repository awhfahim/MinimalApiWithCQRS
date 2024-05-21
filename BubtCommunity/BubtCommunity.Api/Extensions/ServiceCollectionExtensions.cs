using System.Reflection;
using System.Text;
using Asp.Versioning;
using BubtCommunity.Api.EndPoints;
using BubtCommunity.Persistence;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace BubtCommunity.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void BindOptions<TOptions>(this IServiceCollection services,
        string sectionName) where TOptions : class
    {
        services.AddOptions<TOptions>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    public static IServiceCollection AddApiVersioningExtension(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1); //Default version
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(), //Api version from URL
                new HeaderApiVersionReader("X-Api-Version")); //Read Available version from header
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V"; //Format of version
            options.SubstituteApiVersionInUrl = true;
        });
        
        return services;
    }
    public static IServiceCollection AddEndPoints(this IServiceCollection services, Assembly assembly)
    {
        var serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract:false, IsInterface:false} 
                           && type.IsAssignableTo(typeof(IEndPoint)))
            .Select(x => ServiceDescriptor.Transient(typeof(IEndPoint), x))
            .ToList();
        
        services.TryAddEnumerable(serviceDescriptors);
        return services;
    }
    
    public static WebApplication MapEndPoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
    {
        var endPoints = app.Services.GetRequiredService<IEnumerable<IEndPoint>>();
        
        IEndpointRouteBuilder routeBuilder = routeGroupBuilder is null ? app : routeGroupBuilder;
        foreach (var endPoint in endPoints)
        {
            endPoint.MapEndpoint(routeBuilder);
        }

        return app;
    }
    public static void AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:3000"));
        });
    }
    
    #region Copilot Suggested Codes
 // public static void AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    // {
    //     services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //         .AddJwtBearer(options =>
    //         {
    //             options.TokenValidationParameters = new TokenValidationParameters
    //             {
    //                 ValidateIssuerSigningKey = true,
    //                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
    //                 ValidateIssuer = false,
    //                 ValidateAudience = false
    //             };
    //         });
    // }
    //
    // public static void AddCustomAuthorization(this IServiceCollection services)
    // {
    //     services.AddAuthorization(options =>
    //     {
    //         options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    //         options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
    //     });
    // }
    //
    // public static void AddCustomSwagger(this IServiceCollection services)
    // {
    //     services.AddSwaggerGen(c =>
    //     {
    //         c.SwaggerDoc("v1", new OpenApiInfo { Title = "BubtCommunity.Api", Version = "v1" });
    //         c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //         {
    //             Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
    //             Name = "Authorization",
    //             In = ParameterLocation.Header,
    //             Type = SecuritySchemeType.ApiKey,
    //             Scheme = "Bearer"
    //         });
    //         c.AddSecurityRequirement(new OpenApiSecurityRequirement
    //         {
    //             {
    //                 new OpenApiSecurityScheme
    //                 {
    //                     Reference = new OpenApiReference
    //                     {
    //                         Type = ReferenceType.SecurityScheme,
    //                         Id = "Bearer"
    //                     }
    //                 },
    //                 Array.Empty<string>()
    //             }
    //         });
    //     });
    // }
    
    // public static void AddCustomHealthCheck(this IServiceCollection services)
    // {
    //     services.AddHealthChecks()
    //         .AddDbContextCheck<ApplicationDbContext>();
    // }
    

    #endregion
   
    
}