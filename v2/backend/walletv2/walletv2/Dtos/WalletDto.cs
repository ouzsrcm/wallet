namespace walletv2.Dtos;

/// <summary>
/// create account request DTO.
/// </summary>
public class CreateAccountRequest : BaseRequestDto
{
    /// <summary>
    /// Currency ID for the account.
    /// </summary>
    public Guid CurrencyId { get; set; }

    /// <summary>
    /// User ID for the account owner.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Name of the account.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// response DTO for creating an account.
/// </summary>
public class CreateAccountResponse : BaseResponseDto
{
    /// <summary>
    /// ID of the created account.
    /// </summary>
    public Guid AccountId { get; set; }
}

/// <summary>
/// create income/expense request DTO.
/// </summary>
public class CreateIncomeExpenseRequest : BaseRequestDto
{
    /// <summary>
    /// User ID for the income/expense.
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Income/Expense type ID.
    /// </summary>
    public Guid IncomeExpenseTypeId { get; set; }
    /// <summary>
    /// Parent ID for hierarchical structure.
    /// </summary>
    public Guid? ParentId { get; set; }
    /// <summary>
    /// Description of the income/expense.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Icon representing the income/expense.
    /// </summary>
    public string Icon { get; set; } = string.Empty;
}

/// <summary>
/// create income/expense response DTO.
/// </summary>
public class CreateIncomeExpenseResponse : BaseResponseDto
{
    /// <summary>
    /// ID of the created income/expense.
    /// </summary>
    public Guid IncomeExpenseId { get; set; }
}