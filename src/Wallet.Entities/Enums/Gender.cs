using System.ComponentModel;

namespace Wallet.Entities.Enums;

public enum Gender
{
    [Description("Belirtilmemiş")]
    Unspecified = 0,
    
    [Description("Erkek")]
    Male = 1,
    
    [Description("Kadın")]
    Female = 2
} 