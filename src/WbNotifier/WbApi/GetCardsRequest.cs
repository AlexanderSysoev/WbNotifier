namespace WbNotifier.WbApi;

public class GetCardsRequest
{
    public CardsSort Sort { get; set; } = new();
}

public class CardsSort
{
    public CardsCursor Cursor { get; set; } = new();

    public CardsFilter Filter { get; set; } = new();
}

public class CardsCursor
{
    public int Limit { get; set; } = 10;
}

public class CardsFilter
{
    public string TextSearch { get; set; } = string.Empty;

    public int WithPhoto { get; set; } = -1;
}