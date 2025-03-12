using System.ComponentModel;

namespace Wallet.Entities.Enums;

public enum LanguageProficiency
{
    [Description("Başlangıç")]
    Beginner = 1,
    
    [Description("Orta")]
    Intermediate = 2,
    
    [Description("İleri")]
    Advanced = 3,
    
    [Description("Anadil")]
    Native = 4
} 