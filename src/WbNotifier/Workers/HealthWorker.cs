using Cronos;
using Telegram.Bot;
using Telegram.Bot.Types;
using WbNotifier.Settings;
using Timer = System.Timers.Timer;

namespace WbNotifier.Workers;

public class HealthWorker : IHostedService
{
    private Timer? _timer;
    
    private readonly CronExpression _cronExpression;
    private readonly TimeZoneInfo _timeZoneInfo;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ILogger<HealthWorker> _logger;
    private readonly ChatId _chatId;

    public HealthWorker(
        HealthCheckSettings healthCheckSettings,
        ITelegramBotClient telegramBotClient,
        TelegramBotSettings telegramBotSettings,
        ILogger<HealthWorker> logger)
    {
        _cronExpression = CronExpression.Parse(healthCheckSettings.CronExpression);
        _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(healthCheckSettings.TimeZoneId);
        _telegramBotClient = telegramBotClient;
        _logger = logger;
        _chatId = new ChatId(telegramBotSettings.ChatId);
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    { 
        return ScheduleJob(cancellationToken);
    }
    
    private async Task ScheduleJob(CancellationToken cancellationToken)
    {
        var next = _cronExpression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
        if (next.HasValue)
        {
            var delay = next.Value - DateTimeOffset.Now;
            if (delay.TotalMilliseconds <= 0)
            {
                await ScheduleJob(cancellationToken);
            }
            
            _timer = new Timer(delay.TotalMilliseconds);
            _timer.Elapsed += async (sender, args) =>
            {
                _timer.Dispose();
                _timer = null;

                if (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await _telegramBotClient.SendTextMessageAsync(_chatId, "Healthy", cancellationToken: cancellationToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error occured while sending telegram message");
                    }
                }

                if (!cancellationToken.IsCancellationRequested)
                {
                    await ScheduleJob(cancellationToken);
                }
            };
            
            _timer.Start();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Stop();
        return Task.CompletedTask;
    }
}