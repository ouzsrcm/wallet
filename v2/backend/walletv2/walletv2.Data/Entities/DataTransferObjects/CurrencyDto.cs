namespace walletv2.Data.Entities.DataTransferObjects;

public class CurrencyDto
{
    public Guid Id { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string CurrencyName { get; set; } = string.Empty;
    public bool IsLocal { get; set; }
}

public class ExchangeRateDto
{
    public string? CurrencyCode { get; set; }
    public string? CurrencyName { get; set; }
    public bool IsLocal { get; set; }
    public Guid CurrencyId { get; set; }
    public Guid ExchangeRateTypeId { get; set; }
    public string? ExchangeRateTypeName { get; set; }
    public decimal Rate { get; set; }
    public DateTime RateDate { get; set; }
}
