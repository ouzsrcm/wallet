using Wallet.Entities.Base.Concrete;

namespace Wallet.Entities.EntityObjects;

public class Nationality : SoftDeleteEntity
{
    public required string Code { get; set; }  // ISO 3166-1 alpha-2 (TR, US, GB)
    public required string Name { get; set; }  // Turkey, United States, United Kingdom
    public string? NativeName { get; set; }    // Türkiye, United States, United Kingdom
    public string? Alpha3Code { get; set; }    // ISO 3166-1 alpha-3 (TUR, USA, GBR)
    public string? NumericCode { get; set; }   // ISO 3166-1 numeric (792, 840, 826)
    public string? PhoneCode { get; set; }     // +90, +1, +44
    public string? Capital { get; set; }       // Ankara, Washington D.C., London
    public string? Region { get; set; }        // Asia, Americas, Europe
    public string? SubRegion { get; set; }     // Western Asia, Northern America, Northern Europe
    public string? FlagUrl { get; set; }       // URL to flag image
    public bool IsActive { get; set; }
} 