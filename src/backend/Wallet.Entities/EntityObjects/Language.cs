using Wallet.Entities.Base.Concrete;

namespace Wallet.Entities.EntityObjects;

public class Language : SoftDeleteEntity
{
    public required string Code { get; set; }        // ISO 639-1 (tr, en, de)
    public required string Name { get; set; }        // Turkish, English, German
    public string? NativeName { get; set; }          // Türkçe, English, Deutsch
    public string? FlagUrl { get; set; }             // URL to flag image
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public string? LocalizationCode { get; set; }    // tr-TR, en-US, de-DE
    public string? DateFormat { get; set; }          // dd.MM.yyyy, MM/dd/yyyy
    public string? TimeFormat { get; set; }          // HH:mm, hh:mm tt
    public string? CurrencyFormat { get; set; }      // ₺#,##0.00, $#,##0.00
    public string? NumberFormat { get; set; }        // #,##0.00, #.##0,00

    // Navigation Properties
    public ICollection<PersonLanguage>? Persons { get; set; }
} 