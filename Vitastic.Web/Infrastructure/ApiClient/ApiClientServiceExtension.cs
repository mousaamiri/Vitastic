namespace Vitastic.Web.Infrastructure.ApiClient;

public static class ApiClientServiceExtension
{
    public static IServiceCollection AddApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration["ApiSettings:BaseUrl"]
                      ?? throw new ArgumentNullException("ApiSettings:BaseUrl is not configured");

        var timeout = configuration.GetValue<int>("ApiSettings:TimeoutSeconds", 30);

        services.AddHttpContextAccessor();

        services.AddHttpClient<IApiClient, ApiClient>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(timeout);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Accept-Language", "fa-IR");
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip
                                         | System.Net.DecompressionMethods.Deflate
            });

        return services;
    }
}
