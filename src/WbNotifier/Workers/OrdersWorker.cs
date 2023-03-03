using Refit;
using WbNotifier.WbApi;

namespace WbNotifier.Workers;

public class OrdersWorker : BackgroundService
{
    private readonly IWbSuppliersApi _wbSuppliersApi;
    private readonly Notifier _notifier;
    private readonly ILogger<OrdersWorker> _logger;

    public OrdersWorker(
        IWbSuppliersApi wbSuppliersApi,
        Notifier notifier,
        ILogger<OrdersWorker> logger)
    {
        _wbSuppliersApi = wbSuppliersApi;
        _notifier = notifier;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            GetNewOrdersResponse? ordersResponse = null;
            try
            {
                ordersResponse = await _wbSuppliersApi.GetNewOrdersAsync();
            }
            catch (ApiException e)
            {
                _logger.LogError("Error occured while calling Wb suppliers API: returned status code {statusCode} {reason} with content {content}",
                    e.StatusCode, e.ReasonPhrase, e.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured while calling Wb suppliers API");
            }
            
            if (ordersResponse?.Orders != null && ordersResponse.Orders.Any())
            {
                foreach (var order in ordersResponse.Orders)
                {
                    if (order.Skus == null || !order.Skus.Any())
                    {
                        _logger.LogError("No barcodes found in order ID {id}", order.Id);
                        continue;
                    }

                    var barcode = order.Skus.FirstOrDefault();
                    if (string.IsNullOrEmpty(barcode))
                    {
                        _logger.LogError("Empty barcode in order ID {id}", order.Id);
                        continue;
                    }

                    await _notifier.Notify(barcode, $"🔔 New ORDER in total {order.ConvertedPrice / 100} RUB!");
                }
            }
            
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}