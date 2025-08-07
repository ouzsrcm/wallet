using System.Xml.Serialization;

namespace walletv2.Data.Entities.Models;

[XmlRoot("Tarih_Date")]
public class TarihDate
{
    [XmlAttribute("Tarih")]
    public string Tarih { get; set; } = "";

    [XmlAttribute("Date")]
    public string Date { get; set; } = "";

    [XmlAttribute("Bulten_No")]
    public string BultenNo { get; set; } = "";

    [XmlElement("Currency")]
    public List<Currency> Currencies { get; set; } = new();
}

public class Currency
{
    [XmlAttribute("CrossOrder")]
    public string? CrossOrder { get; set; }

    [XmlAttribute("Kod")]
    public string Kod { get; set; } = "";

    [XmlAttribute("CurrencyCode")]
    public string CurrencyCode { get; set; } = "";

    [XmlElement("Unit")]
    public int? Unit { get; set; }

    [XmlElement("Isim")]
    public string Isim { get; set; } = "";

    [XmlElement("ForexBuying")]
    public string? ForexBuying { get; set; }

    [XmlElement("ForexSelling")]
    public string? ForexSelling { get; set; }

    [XmlElement("BanknoteBuying")]
    public string? BanknoteBuying { get; set; }

    [XmlElement("BanknoteSelling")]
    public string? BanknoteSelling { get; set; }
}
