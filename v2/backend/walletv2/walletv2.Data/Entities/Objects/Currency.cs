using System.ComponentModel.DataAnnotations;

namespace walletv2.Data.Entities.Objects;

public class Currency : BaseEntityImplementation
{
    [Required]
    [MinLength(3)]
    [MaxLength(3)]
    public required string CurrencyCode { get; set; }

    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public required string CurrencyName { get; set; }

    [Required]
    public bool IsLocal { get; set; }
}

public class ExchangeRateType : BaseEntityImplementation
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public required string TypeName { get; set; }
}

public class ExchangeRate : BaseEntityImplementation
{
    [Required]
    public Guid CurrencyId { get; set; }

    [Required]
    public Guid ExchangeRateTypeId { get; set; }

    public decimal Rate { get; set; }
    public DateTime RateDate { get; set; }

    public virtual Currency? Currency { get; set; }
    public virtual ExchangeRateType? ExchangeRateType { get; set; }
}
