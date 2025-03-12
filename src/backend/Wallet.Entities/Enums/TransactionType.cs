using System.ComponentModel;

namespace Wallet.Entities.Enums;

public enum TransactionType
{
    [Description("Gelir")]
    Income = 1,

    
    [Description("Gider")]
    Expense = 2
} 