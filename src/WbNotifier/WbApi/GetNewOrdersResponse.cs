namespace WbNotifier.WbApi;

public class GetNewOrdersResponse
{
    /// <summary>
    /// Список заказов
    /// </summary>
    public List<Order>? Orders { get; set; }
}

public class Order
{
    /// <summary>
    /// Идентификатор сборочного задания в Маркетплейсе
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Массив штрихкодов товара
    /// </summary>
    public List<string>? Skus { get; set; }
    
    /// <summary>
    /// Цена в валюте продажи с учетом всех скидок, сконвертированная по курсу на момент продажи в российские копейки.
    /// Предоставляется в информационных целях.
    /// </summary>
    public decimal ConvertedPrice { get; set; }
}