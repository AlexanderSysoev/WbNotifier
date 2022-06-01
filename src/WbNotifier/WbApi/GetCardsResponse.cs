namespace WbNotifier.WbApi;

public class GetCardsResponse
{
    public string Id { get; set; }

    public string Jsonrpc { get; set; }

    public CardsList Result { get; set; }
}

public class CardsList
{
    public List<Card> Cards { get; set; }

    public Cursor Cursor { get; set; }
}

public class Card
{
    /// <summary>
    /// Общие характеристики товара
    /// </summary>
    public List<Addin> Addin { get; set; }

    /// <summary>
    /// Страна-изготовитель товара
    /// </summary>
    public string CountryProduction { get; set; }

    /// <summary>
    /// Дата создания карточки. Заполняется автоматически
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Идентификатор карточки. Генерируется при создании карточки
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Структура с характеристиками номенклатур товара
    /// </summary>
    public List<Nomenclature2> Nomenclatures { get; set; }

    /// <summary>
    /// Продукт. К какой категории принадлежит товар (джинсы, книги и прочее)
    /// </summary>
    public string Object { get; set; }

    /// <summary>
    /// Родительская категория
    /// </summary>
    public string Parent { get; set; }

    /// <summary>
    /// Идентификатор поставщика. Подставляется автоматически
    /// </summary>
    public string SupplierId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string SupplierVendorCode { get; set; }

    /// <summary>
    /// Дата последнего обновления карточки. Заполняется автоматически
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Айди массовой загрузки, в процессе которой была создана карточка. Если айди пуст, то карточка была создана другим способом
    /// </summary>
    public string UploadID { get; set; }

    /// <summary>
    /// Идентификатор пользователя, создавшего карточку. Подставляется автоматически
    /// </summary>
    public int UserId { get; set; }
}

public class Addin
{
    public List<Parameter> Params { get; set; }

    public string Type { get; set; }
}

public class Parameter
{
    public int Count { get; set; }

    public string Units { get; set; }

    public string Value { get; set; }
}

public class Nomenclature2
{
    /// <summary>
    /// Характеристики конкретной номенклатуры товара
    /// </summary>
    public List<Addin> Addin { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ConcatVendorCode { get; set; }

    /// <summary>
    /// Идентификатор цвета товара. Генерируется при создании карточки
    /// </summary>
    public string Id { get; set; }

    public bool IsArchive { get; set; }

    /// <summary>
    /// Структура с характеристиками различных размеров номенклатуры
    /// </summary>
    public List<Variation> Variations { get; set; }

    /// <summary>
    /// Артикул товара
    /// </summary>
    public string VendorCode { get; set; }
}

public class Variation
{
    /// <summary>
    /// Характеристики конкретной вариации номенклатуры
    /// </summary>
    public List<Addin> Addin { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string Barcode { get; set; }

    /// <summary>
    /// Штрихкоды
    /// </summary>
    public List<string> Barcodes { get; set; }

    /// <summary>
    /// Ошибки от старой спеки
    /// </summary>
    public List<string> Errors { get; set; }

    /// <summary>
    /// Идентификатор. Генерируется автоматически
    /// </summary>
    public string Id { get; set; }
}