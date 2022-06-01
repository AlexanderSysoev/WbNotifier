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
            GetOrdersResponse? ordersResponse = null;
            try
            {
                //Uncomment for test purposes
                /*ordersResponse = await _wbSuppliersApi.GetOrdersAsync(
                    dateStart: new DateTime(2022, 5, 30),
                    dateEnd: null,
                    status: OrderStatus._2,
                    take: 10,
                    skip: 0,
                    id: null);*/
                
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
                _logger.LogError("Error occured while calling Wb suppliers API: returned status code {statusCode} {reason} with content {content}",
                    e.StatusCode, e.ReasonPhrase, e.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occured while calling Wb suppliers API");
            }
            
            if (ordersResponse is {Total: > 0})
            {
                foreach (var order in ordersResponse.Orders)
                {
                    await _notifier.Notify(order.Barcode, $"🔔 New ORDER in total {order.TotalPrice / 100} RUB!");
                }
            }
            
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}