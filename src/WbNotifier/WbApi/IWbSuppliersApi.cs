using Refit;

namespace WbNotifier.WbApi;

public interface IWbSuppliersApi
{
    /// <summary>
    /// Возвращает список всех новых сборочных заданий у продавца на данный момент.
    /// </summary>
    /// <returns>Список сборочных заданий</returns>
    [Get("/api/v3/orders/new")]
    public Task<GetNewOrdersResponse?> GetNewOrdersAsync();
    
    /// <summary>
    /// Получить список карточек поставщика
    /// </summary>
    /// <param name="request">Запрос на получение</param>
    /// <returns>Ответ со списком карточек</returns>
    [Post("/content/v1/cards/cursor/list")]
    public Task<GetCardsResponse> GetCardsAsync(GetCardsRequest request);
}