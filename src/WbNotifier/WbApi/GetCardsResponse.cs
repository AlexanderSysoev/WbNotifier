namespace WbNotifier.WbApi;

public class GetCardsResponse
{
    public GetCardsData Data { get; set; } = new();
}

public class GetCardsData
{
    public List<Card> Cards { get; set; } = new();
}

public class Card
{
    public List<Size> Sizes { get; set; } = new();

    public List<string> MediaFiles { get; set; } = new();

    public List<string> Colors { get; set; } = new();

    public string VendorCode { get; set; } = string.Empty;
}

public class Size
{
    public string TechSize { get; set; } = string.Empty;

    public List<string> Skus { get; set; } = new();
}