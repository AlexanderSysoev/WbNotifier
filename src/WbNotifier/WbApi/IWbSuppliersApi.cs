using Refit;

namespace WbNotifier.WbApi;

public interface IWbSuppliersApi
{
    /// <summary>
    /// Возвращает список сборочных заданий поставщика.
    /// </summary>
    /// <param name="dateStart">С какой даты вернуть сборочные задания (заказы) (в формате RFC3339)</param>
    /// <param name="dateEnd">По какую дату вернуть сборочные задания (заказы) (в формате RFC3339)</param>
    /// <param name="status">Заказы какого статуса нужны</param>
    /// <param name="take">Сколько записей вернуть за раз</param>
    /// <param name="skip">Сколько записей пропустить</param>
    /// <param name="id">Идентификатор сборочного задания, если нужно получить данные по какому-то определенному заказу.</param>
    /// <returns>Список сборочных заданий</returns>
    [Get("/api/v2/orders")]
    public Task<GetOrdersResponse> GetOrdersAsync(
        [Query(Format = "yyyy-MM-dd'T'HH:mm:ss.fffK")][AliasAs("date_start")]DateTimeOffset dateStart,
        [Query(Format = "yyyy-MM-dd'T'HH:mm:ss.fffK")][AliasAs("date_end")] DateTimeOffset? dateEnd,
        OrderStatus? status, int take, int skip, int? id);
    
    /// <summary>
    /// Получить список карточек поставщика
    /// </summary>
    /// <param name="request">Запрос на получение</param>
    /// <returns>Ответ со списком карточек</returns>
    [Post("/content/v1/cards/cursor/list")]
    public Task<GetCardsResponse> GetCardsAsync(GetCardsRequest request);
}