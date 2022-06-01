using Refit;

namespace WbNotifier.WbApi;

public interface IWbStatsApi
{
    /// <summary>
    /// Возвращает список продаж, отрицательная сумма означает возврат
    /// </summary>
    /// <param name="dateFrom">Дата поиска</param>
    /// <param name="flag">Флаг. Значение 0 - возвращаются данные у которых значение поля lastChangeDate
    /// (дата время обновления информации в сервисе) больше переданного параметра dateFrom. Значение 1 -
    /// возвращаются данные обо всех продажах с датой равной переданному параметру dateFrom</param>
    /// <returns>Список продаж</returns>
    [Get("/api/v1/supplier/sales")]
    public Task<List<Sale>> GetSales([Query(Format = "yyyy-MM-dd")]DateTime dateFrom, int flag);
}

public class Sale : IEquatable<Sale>
{
    public Sale(string? saleId, DateTime date, string? supplierArticle, decimal finishedPrice, decimal forPay, string? subject,
        string? brand, string? warehouseName, string? barcode)
    {
        SaleId = saleId;
        Date = date;
        SupplierArticle = supplierArticle;
        FinishedPrice = finishedPrice;
        ForPay = forPay;
        Subject = subject;
        Brand = brand;
        WarehouseName = warehouseName;
        Barcode = barcode;
    }
    
    /// <summary>
    /// Уникальный идентификатор продажи/возврата
    /// </summary>
    public string? SaleId { get; }

    /// <summary>
    /// Баркод
    /// </summary>
    public string? Barcode { get; }
    
    /// <summary>
    /// Дата продажи
    /// </summary>
    public DateTime Date { get; }

    /// <summary>
    /// Артикул поставщика
    /// </summary>
    public string? SupplierArticle { get; }

    /// <summary>
    /// Фактическая цена из заказа (с учетом всех скидок, включая и от ВБ)
    /// </summary>
    public decimal FinishedPrice { get; }

    /// <summary>
    /// Сумма к перечислению поставщику
    /// </summary>
    public decimal ForPay { get; }

    /// <summary>
    /// Предмет
    /// </summary>
    public string? Subject { get; }

    /// <summary>
    /// Брэнд
    /// </summary>
    public string? Brand { get; }

    /// <summary>
    /// Склад
    /// </summary>
    public string? WarehouseName { get; }

    public bool Equals(Sale? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Date.Equals(other.Date) && SupplierArticle == other.SupplierArticle &&
               FinishedPrice == other.FinishedPrice && ForPay == other.ForPay && Subject == other.Subject &&
               Brand == other.Brand && WarehouseName == other.WarehouseName && Barcode == other.Barcode;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Sale) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Date, SupplierArticle, FinishedPrice, ForPay, Subject, Brand, WarehouseName, Barcode);
    }
}