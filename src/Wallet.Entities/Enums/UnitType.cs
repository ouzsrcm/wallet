using System.ComponentModel;

namespace Wallet.Entities.Enums;

public enum UnitType
{
    [Description("Adet")]
    Piece = 1,      // Adet
    [Description("Kilogram")]
    Kilogram = 2,   // Kilogram
    [Description("Gram")]
    Gram = 3,       // Gram
    [Description("Litre")]
    Liter = 4,      // Litre
    [Description("Mililitre")]
    Milliliter = 5, // Mililitre
    [Description("Metre")]
    Meter = 6,      // Metre
    [Description("Kutu")]
    Box = 7,        // Kutu
    [Description("Paket")]
    Pack = 8,       // Paket
    [Description("Diğer")]
    Other = 99      // Diğer
} 