using Microsoft.AspNetCore.Mvc;
using Wallet.Entities.Enums;
using System.Collections.Generic;
using System.Linq;
using Wallet.Entities.Extensions;
using Wallet.Entities.Enums;

namespace Wallet.Api.Controllers;

[ApiController]
[Produces("application/json")]
[ApiVersion("1.0")]
[Tags("Enums")]
[Route("api/[controller]")]
public class EnumsController : ControllerBase
{
    private readonly ILogger<EnumsController> _logger;

    public EnumsController(ILogger<EnumsController> logger)
    {
        _logger = logger;
    }

    [HttpGet("transaction-types")]
    [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
    public IActionResult GetTransactionTypes()
    {
        var transactionTypes = Enum.GetValues(typeof(TransactionType))
            .Cast<TransactionType>()
            .Select(e => new
            {
                Id = (int)e,
                Name = e.ToString(),
                Title = e.GetDescription()
            })
            .ToList();

        return Ok(transactionTypes);
    }

    [HttpGet("payment-methods")]
    [ProducesResponseType(typeof(List<object>), StatusCodes.Status200OK)]
    public IActionResult GetPaymentMethods()
    {
        var paymentMethods = Enum.GetValues(typeof(PaymentMethod))
            .Cast<PaymentMethod>()
            .Select(e => new
            {
                Id = (int)e,
                Name = e.ToString(),
                Title = e.GetDescription()
            })
            .ToList();

        return Ok(paymentMethods);
    }


}