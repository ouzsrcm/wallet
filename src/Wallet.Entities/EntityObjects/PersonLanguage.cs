using Wallet.Entities.Base.Concrete;
using Wallet.Entities.Enums;

namespace Wallet.Entities.EntityObjects;

public class PersonLanguage : SoftDeleteEntity
{
    public Guid PersonId { get; set; }
    public Guid LanguageId { get; set; }
    public bool IsPrimary { get; set; }
    public LanguageProficiency ProficiencyLevel { get; set; }

    // Navigation properties
    public Person Person { get; set; } = null!;
    public Language Language { get; set; } = null!;
} 