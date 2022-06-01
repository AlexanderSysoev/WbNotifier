namespace WbNotifier.Settings;

public class TelegramBotSettings
{
    public string Host { get; set; } = default!;

    public long ChatId { get; set; } = default;
    
    public string Token { get; set; } = default!;
}