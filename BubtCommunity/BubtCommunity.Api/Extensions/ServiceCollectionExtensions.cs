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
}