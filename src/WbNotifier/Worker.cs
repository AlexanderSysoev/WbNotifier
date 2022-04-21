using Refit;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WbNotifier;

public class Worker : BackgroundService
{
    private readonly IWbSuppliersApi _wbSuppliersApi;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ChatId _chatId;
    private readonly ILogger<Worker> _logger;

    public Worker(IWbSuppliersApi wbSuppliersApi,
        ITelegramBotClient telegramBotClient,
        TelegramBotSettings telegramBotSettings,
        ILogger<Worker> logger)
    {
        _wbSuppliersApi = wbSuppliersApi;
        _telegramBotClient = telegramBotClient;
        _chatId = new ChatId(telegramBotSettings.ChatId);
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            OrdersResponse? ordersResponse = null;
            try
            {
                ordersResponse = await _wbSuppliersApi.GetOrdersAsync(
                    dateStart: DateTimeOffset.Now.AddHours(-24),
                    dateEnd: null,
                    status: OrderStatus._0,
                    take: 10,
                    skip: 0,
                    id: null);
            }
            catch (ApiException e)
            {
                _logger.LogError("Error occured while calling Wb Suppliers API: returned status code {statusCode} {reason} with content {content}",
                    e.StatusCode, e.ReasonPhrase, e.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured while calling Wb Suppliers API");
            }
            
            if (ordersResponse is {Total: > 0})
            {
                try
                {
                    await _telegramBotClient.SendTextMessageAsync(_chatId, $"New orders detected: {ordersResponse.Total} count!", cancellationToken: stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error occured while sending telegram message");
                }
            }
            
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}