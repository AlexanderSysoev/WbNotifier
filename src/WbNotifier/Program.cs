using System.Net.Http.Headers;
using Refit;
using Serilog;
using Serilog.Events;
using Telegram.Bot;
using WbNotifier;
using WbNotifier.Logging;
using WbNotifier.Serialization;
using WbNotifier.Settings;
using WbNotifier.WbApi;
using WbNotifier.Workers;

Log.Logger = new LoggerConfiguration()
    .CreateLogger();

var hostBuilder = Host.CreateDefaultBuilder(args);

var host = hostBuilder
    .ConfigureServices((hostBuilderContext, services) =>
    {
        var wbOrdersApiSettings = BindSettings<WbSuppliersApiSettings>("WbSuppliersApi");
        var wbSalesApiSettings = BindSettings<WbStatsApiSettings>("WbStatsApi");
        var telegramBotSettings = BindSettings<TelegramBotSettings>("TelegramBot");
        BindSettings<HealthCheckSettings>("HealthCheck");

        services.AddHostedService<OrdersWorker>();
        services.AddHostedService<SalesWorker>();
        services.AddHostedService<HealthWorker>();
        services.AddSingleton<Notifier>();

        services.AddTransient<HttpLoggingHandler>();
        services.AddTransient<ApiKeyInjectorDelegatingHandler>();
        services.AddRefitClient<IWbSuppliersApi>(new RefitSettings
            {
                UrlParameterFormatter = new EnumAsIntParameterFormatter()
            })
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(wbOrdersApiSettings.Host);
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", wbOrdersApiSettings.Token);
            })
            .AddHttpMessageHandler<HttpLoggingHandler>();
        
        services.AddRefitClient<IWbStatsApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(wbSalesApiSettings.Host);
            })
            .AddHttpMessageHandler<ApiKeyInjectorDelegatingHandler>()
            .AddHttpMessageHandler<HttpLoggingHandler>();
        
        services.AddHttpClient("telegram")
            .AddTypedClient<ITelegramBotClient>(client =>
            {
                client.BaseAddress = new Uri(telegramBotSettings.Host);
                return new TelegramBotClient(telegramBotSettings.Token, client);
            });

        T BindSettings<T>(string sectionName) where T : class, new()
        {
            var settings = new T();
            hostBuilderContext.Configuration.GetSection(sectionName).Bind(settings);
            services.AddSingleton(settings);

            return settings;
        }
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