using Asp.Versioning;
using Asp.Versioning.Builder;
using BubtCommunity.Api.Extensions;
using BubtCommunity.Api.Options;
using BubtCommunity.Persistence;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Scrutor;
using Serilog;
using Serilog.Events;

Env.Load();

var configuration = new ConfigurationBuilder()
   // .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    // Add Scrutor
    builder
        .Services
        .Scan(
            selector => selector
                .FromAssemblies(
                    BubtCommunity.Infrastructure.AssemblyReference.Assembly,
                    BubtCommunity.Persistence.AssemblyReference.Assembly)
                .AddClasses(false)
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithScopedLifetime());
    
    builder.Services.BindOptions<SerilogEmailSinkOptions>(SerilogEmailSinkOptions.SectionName);
    builder.Services.BindOptions<ConnectionStringOptions>(ConnectionStringOptions.SectionName);
    
    builder.Services.AddSerilog((_, lc) => lc
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(configuration)
        .WriteTo.ConfigureEmailSink(builder.Configuration)
    );
    //Add Api Versioning
    builder.Services.AddApiVersioningExtension();

    //Add MediatR
    builder.Services.AddMediatR(cfg => 
        cfg.RegisterServicesFromAssembly(BubtCommunity.Application.AssemblyReference.Assembly));
    
    builder.Services.AddEndPoints(typeof(Program).Assembly);

    // Add services to the container.
    var connectionString= builder.Configuration
        .GetRequiredSection(ConnectionStringOptions.SectionName)
        .GetValue<string>(nameof(ConnectionStringOptions.BubtCommunityDb));

    var migrationAssembly = typeof(ApplicationDbContext).Assembly.GetName().Name;
    
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(connectionString, 
                m => m.MigrationsAssembly(migrationAssembly))
            .UseEnumCheckConstraints();
        
        ArgumentNullException.ThrowIfNull(connectionString, nameof(ConnectionStringOptions));
    });
    
    
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();
    
    //Configure API Versioning
    ApiVersionSet apiVersionSet = app.NewApiVersionSet()
        .HasApiVersion(new ApiVersion(1)) //Add version 1
        .HasApiVersion(new ApiVersion(2)) //Add version 2
        .ReportApiVersions() // Report available versions
        .Build();

    //Create versioned group for API
    RouteGroupBuilder versionedGroup = app
        .MapGroup("api/v{version:apiVersion}") //add version to URL
        .WithApiVersionSet(apiVersionSet); //Add version set to group

    app.MapEndPoints(versionedGroup);

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    
    await app.RunAsync();
    return 0;
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application start-up failed");
    return 1;
}
finally
{
    Log.Information("Shut down complete");
    await Log.CloseAndFlushAsync();
}