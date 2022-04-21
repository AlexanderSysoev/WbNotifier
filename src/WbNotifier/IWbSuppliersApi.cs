using Refit;

namespace WbNotifier;

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
    public Task<OrdersResponse> GetOrdersAsync(
        [Query(Format = "yyyy-MM-dd'T'HH:mm:ss.fffK")][AliasAs("date_start")]DateTimeOffset dateStart,
        [Query(Format = "yyyy-MM-dd'T'HH:mm:ss.fffK")][AliasAs("date_end")] DateTimeOffset? dateEnd,
        OrderStatus? status, int take, int skip, int? id);
}

public class OrdersResponse
{
    /// <summary>
    /// Общее количество заказов по заданным параметрам (за указанный промежуток времени)
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Список заказов
    /// </summary>
    public List<Order> Orders { get; set; }
}

public class Order
{
    /// <summary>
    /// Идентификатор заказа
    /// </summary>
    public string OrderId { get; set; }

    /// <summary>
    /// Дата создания заказа (RFC3339)
    /// </summary>
    public DateTimeOffset DateCreated { get; set; }

    /// <summary>
    /// Идентификатор склада WB, на которой заказ должен быть доставлен
    /// </summary>
    public int WbWhId { get; set; }

    /// <summary>
    /// Идентификатор склада поставщика, на который пришел заказ.
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Идентификатор ПВЗ/магазина, куда необходимо доставить заказ (если применимо)
    /// </summary>
    public int Pid { get; set; }

    /// <summary>
    /// Адрес ПВЗ/магазина, куда необходимо доставить заказ (если применимо)
    /// </summary>
    public string OfficeAddress { get; set; }

    /// <summary>
    /// Широта адреса ПВЗ/магазина, куда необходимо доставить заказ (если применимо)
    /// </summary>
    public double OfficeLatitude { get; set; }

    /// <summary>
    /// Долгота адреса ПВЗ/магазина, куда необходимо доставить заказ (если применимо)
    /// </summary>
    public double OfficeLongitude { get; set; }

    /// <summary>
    /// Адрес клиента для доставки
    /// </summary>
    public string DeliveryAddress { get; set; }

    /// <summary>
    /// Детализованный адрес клиента для доставки (если применимо). Некоторые из полей могут прийти пустыми из-за специфики адреса.
    /// </summary>
    public DeliveryAddressDetails DeliveryAddressDetails { get; set; }

    /// <summary>
    /// Идентификатор артикула
    /// </summary>
    public int ChrtId { get; set; }

    /// <summary>
    /// Штрихкод товара
    /// </summary>
    public string Barcode { get; set; }

    /// <summary>
    /// Массив штрихкодов товара
    /// </summary>
    public List<string> Barcodes { get; set; }

    /// <summary>
    /// Массив СЦ приоритетных для доставки заказа. Если поле не заполнено или массив пустой, приоритетного СЦ нет для данного заказа нет.
    /// </summary>
    public List<string> ScOfficesNames { get; set; }

    /// <summary>
    ///  0	- Новый заказ
    /// <br/>
    /// <br/> 1	- Принял заказ
    /// <br/>
    /// <br/> 2	- Сборочное задание завершено
    /// <br/>
    /// <br/> 3	- Сборочное задание отклонено
    /// <br/>
    /// <br/> 5 - На доставке курьером
    /// <br/>
    /// <br/> 6 - Курьер довез и клиент принял товар
    /// <br/>
    /// <br/> 7 - Клиент не принял товар
    /// <br/>
    /// <br/> 8 - Товар для самовывоза из магазина принят к работе
    /// <br/>
    /// <br/> 9 - Товар для самовывоза из магазина готов к выдаче
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// 1 - Отмена клиента 2 - Доставлен 3 - Возврат 4 - Ожидает 5 - Брак
    /// <br/>
    /// </summary>
    public OrderUserStatus UserStatus { get; set; }

    /// <summary>
    /// Уникальный идентификатор вещи, разный у одинаковых chrt_id
    /// </summary>
    public string Rid { get; set; }

    /// <summary>
    /// Стоимость товара с учетом скидок в копейках!
    /// </summary>
    public int TotalPrice { get; set; }

    /// <summary>
    /// Идентификатор транзакции для группировки заказов. Заказы в одной корзине клиента будут иметь одинаковый orderUID.
    /// </summary>
    public string OrderUID { get; set; }

    /// <summary>
    /// Тип доставки:
    /// <br/>* 1 -  обычная доставка
    /// <br/>* 2 -  доставка силами поставщика
    /// <br/>
    /// </summary>
    public OrderDeliveryType DeliveryType { get; set; }
}

public class DeliveryAddressDetails
{
    /// <summary>
    /// Область
    /// </summary>
    public string Province { get; set; }

    /// <summary>
    /// Район
    /// </summary>
    public string Area { get; set; }

    /// <summary>
    /// Город
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Улица
    /// </summary>
    public string Street { get; set; }

    /// <summary>
    /// Номер дома
    /// </summary>
    public string Home { get; set; }

    /// <summary>
    /// Номер квартиры
    /// </summary>
    public string Flat { get; set; }

    /// <summary>
    /// Подъезд
    /// </summary>
    public string Entrance { get; set; }

    /// <summary>
    /// Координата долготы
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Координата широты
    /// </summary>
    public double Latitude { get; set; }
}

public enum OrderStatus
{
    _0 = 0,
    _1 = 1,
    _2 = 2,
    _3 = 3,
    _5 = 5,
    _6 = 6,
    _7 = 7,
    _8 = 8,
    _9 = 9
}

public enum OrderUserStatus
{
    _1 = 1,
    _2 = 2,
    _3 = 3,
    _4 = 4,
    _5 = 5
}

public enum OrderDeliveryType
{
    _1 = 1,
    _2 = 2,
}