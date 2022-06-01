using Refit;
using WbNotifier.WbApi;

namespace WbNotifier.Workers;

public class SalesWorker : BackgroundService
{
    private readonly IWbStatsApi _wbStatsApi;
    private readonly Notifier _notifier;
    private readonly ILogger<SalesWorker> _logger;
    private List<Sale> _previousSales;

    public SalesWorker(
        IWbStatsApi wbStatsApi,
        Notifier notifier,
        ILogger<SalesWorker> logger)
    {
        _wbStatsApi = wbStatsApi;
        _notifier = notifier;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var isFirstRun = true;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            List<Sale>? sales = null;
            try
            {
                //Uncomment for test purposes
                //sales = await _wbStatsApi.GetSales(new DateTime(2022, 4, 4), 1);
                sales = await _wbStatsApi.GetSales(DateTime.Now, 1);
            }
            catch (ApiException e)
            {
                _logger.LogError("Error occured while calling Wb sales API: returned status code {statusCode} {reason} with content {content}",
                    e.StatusCode, e.ReasonPhrase, e.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured while calling Wb sales API");
            }

            if (sales != null)
            {
                if (isFirstRun)
                {
                    _previousSales = sales;
                    isFirstRun = false;
                }
                else
                {
                    var newSales = sales.Except(_previousSales).ToList();
                    foreach (var newSale in newSales)
                    {
                        await _notifier.Notify(newSale.Barcode, $"💰 New SALE in total {newSale.ForPay} RUB!");
                    }
                }
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}