namespace WbNotifier.Settings;

public class HealthCheckSettings
{
    public string CronExpression { get; set; } = default!;
    
    public string TimeZoneId { get; set; } = default!;
}