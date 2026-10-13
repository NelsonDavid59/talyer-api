namespace TalyerApp.Api.Configuration;

public static class HostConfigurationExtensions
{
    public static WebApplicationBuilder AddTalyerDevelopmentUserSecrets(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddJsonFile(
                Path.Combine(builder.Environment.ContentRootPath, "usersecrets.json"),
                optional: true,
                reloadOnChange: false);
        }

        return builder;
    }
}
