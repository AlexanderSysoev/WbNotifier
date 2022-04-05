using System.Net.Http.Headers;
using Refit;
using Serilog;
using Serilog.Events;
using Telegram.Bot;
using WbNotifier;

Log.Logger = new LoggerConfiguration()
    .CreateLogger();

var hostBuilder = Host.CreateDefaultBuilder(args);

var host = hostBuilder
    .ConfigureServices((hostBuilderContext, services) =>
    {
        var wbApiSettings = new WbApiSettings();
        hostBuilderContext.Configuration.GetSection("WildberriesApi").Bind(wbApiSettings);
        services.AddSingleton(wbApiSettings);

        var telegramBotSettings = new TelegramBotSettings();
        hostBuilderContext.Configuration.GetSection("TelegramBot").Bind(telegramBotSettings);
        services.AddSingleton(telegramBotSettings);
        
        var healthCheckSettings = new HealthCheckSettings();
        hostBuilderContext.Configuration.GetSection("HealthCheck").Bind(healthCheckSettings);
        services.AddSingleton(healthCheckSettings);
        
        services.AddHostedService<Worker>();
        services.AddHostedService<HealthService>();
        
        services.AddTransient<HttpLoggingHandler>();
        services.AddRefitClient<IWbSuppliersApi>(new RefitSettings
            {
                UrlParameterFormatter = new EnumAsIntParameterFormatter()
            })
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(wbApiSettings.Host);
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", wbApiSettings.Token);
            })
            .AddHttpMessageHandler<HttpLoggingHandler>();
        
        services.AddHttpClient("telegram")
            .AddTypedClient<ITelegramBotClient>(client =>
            {
                client.BaseAddress = new Uri(telegramBotSettings.Host);
                return new TelegramBotClient(telegramBotSettings.Token, client);
            });
    })
    .UseSerilog(
        (context, services, configuration) =>
        {
            Serilog.Debugging.SelfLog.Enable(Console.Error);
            
            var telegramBotSettings = services.GetService<TelegramBotSettings>();
            configuration.ReadFrom.Configuration(context.Configuration)
                .WriteTo.Telegram(
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    botToken: telegramBotSettings.Token,
                    chatId: telegramBotSettings.ChatId.ToString(),
                    applicationName: "WbNotifier");
        })
    .Build();

await host.RunAsync();