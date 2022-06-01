namespace WbNotifier.WbApi;

public class GetCardsRequest
{
    public string? Id { get; set; }

    public string? Jsonrpc { get; set; }

    public GetCardRequestParams? Params { get; set; }
}

public class GetCardRequestParams
{
    // Фильтр
    public Sort? Filter { get; set; }

    // Пагинация
    public Cursor? Query { get; set; }

    // Параметр указывающий, что вернуться только карточки в которых есть ошибки, которые не удалось создать.
    // Параметр не обязательный, если его не указывать вернуться только созадныне карточки
    public bool WithError { get; set; }
}

public class Sort
{
    public List<Filter>? Filter { get; set; }

    public List<Find>? Find { get; set; }

    public CardOrder? Order { get; set; }
}

public class Filter
{
    public string? Column { get; set; }
    
    public string? ExcludedValues { get; set; }
}

public class Find
{
    public string? Column { get; set; }
    
    public string? Search { get; set; }
}

public class CardOrder
{
    public string? Column { get; set; }

    public string? Order { get; set; }
}

public class Cursor
{
    public int Limit { get; set; }

    public int Offset { get; set; }

    public int Total { get; set; }
}
