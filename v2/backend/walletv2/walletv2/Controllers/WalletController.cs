using Microsoft.AspNetCore.Mvc;
using walletv2.Data.Entities.DataTransferObjects;
using walletv2.Data.Services;
using walletv2.Dtos;
using walletv2.Extensions;

namespace walletv2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        this._walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
    }

    /// <summary>
    /// creates a new account with the provided details.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("account")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CreateAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CreateAccountResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        if (request == null)
        {
            return BadRequest(new CreateAccountResponse
            {
                Status = ApiResponseStatus.Error,
                Message = "Invalid request data."
            });
        }
        try
        {
            var accountId = await _walletService.CreateAccountAsync(new CreateAccountDto()
            {
                CurrencyId = request.CurrencyId,
                UserId = request.UserId,
                Name = request.Name ?? string.Empty
            });
            return Ok(new CreateAccountResponse
            {
                Status = ApiResponseStatus.Success,
                AccountId = accountId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new CreateAccountResponse
            {
                Status = ApiResponseStatus.ServerError,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// creates a new income/expense entry with the provided details.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("income-expense")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CreateIncomeExpenseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CreateIncomeExpenseResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateIncomeExpense([FromBody] CreateIncomeExpenseRequest request)
    {
        if (request == null)
        {
            return BadRequest(new CreateIncomeExpenseResponse
            {
                Status = ApiResponseStatus.Error,
                Message = "Invalid request data."
            });
        }
        try
        {
            var userId = User.GetUserId();
            var incomeExpenseId = await _walletService.CreateIncomeExpenseAsync(new CreateIncomeExpenseDto()
            {
                UserId = userId,
                IncomeExpenseTypeId = request.IncomeExpenseTypeId,
                ParentId = request.ParentId,
                Description = request.Description ?? string.Empty,
                Icon = request.Icon ?? string.Empty
            });
            return Ok(new CreateIncomeExpenseResponse
            {
                Status = ApiResponseStatus.Success,
                IncomeExpenseId = incomeExpenseId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new CreateIncomeExpenseResponse
            {
                Status = ApiResponseStatus.ServerError,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// returns a list of income and expenses for the user.
    /// </summary>
    /// <returns></returns>
    [HttpGet("income-expenses")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IncomeExpenseListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IncomeExpenseListResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetIncomeExpenses()
    {
        try
        {
            var userId = User.GetUserId();
            return Ok(new IncomeExpenseListResponse
            {
                Status = ApiResponseStatus.Success,
                Items = from x in (await _walletService.GetListOfIncomeExpenseAsync(userId)).Items
                        select new IncomeExpenseResponse()
                        {
                            Name = x.Name,
                            Icon = x.Icon,
                            ParentId = x.ParentId,
                            Status = ApiResponseStatus.Success,
                            IncomeExpenseId = x.IncomeExpenseId,
                            IncomeExpenseTypeName = x.IncomeExpenseTypeName,
                            IncomeExpenseTypeDescription = x.IncomeExpenseTypeDescription,
                        }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new IncomeExpenseListResponse
            {
                Status = ApiResponseStatus.ServerError,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// returns a list of income and expense types.
    /// </summary>
    /// <returns></returns>
    [HttpGet("income-expense-types")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IncomeExpenseTypeListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IncomeExpenseTypeListResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetIncomeExpenseTypes()
    {
        try
        {
            return Ok(new IncomeExpenseTypeListResponse()
            {
                Items = (await _walletService.GetIncomeExpenseTypeListAsync()).Items?.Select(x => new IncomeExpenseTypeResponse()
                {
                    Name = x.Name,
                    Description = x.Description,
                    Status = ApiResponseStatus.Success,
                    IncomeExpenseTypeId = x.IncomeExpenseTypeId
                }),
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new IncomeExpenseTypeListResponse
            {
                Status = ApiResponseStatus.ServerError,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// list of the cashflow for the user.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("cashflows")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CashflowListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CashflowListResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCashflows([FromBody] CashflowListRequest model)
    {
        try
        {
            var res = await _walletService.GetCashflowListAsync(new GetCashflowListDto()
            {
                EndDate = model.EndDate,
                UserId = User.GetUserId(),
                StartDate = model.StartDate,
                CurrencyId = model.CurrencyId,
                CashflowTypeId = model.CashflowTypeId,
            });
            return Ok(new CashflowListResponse
            {
                Status = ApiResponseStatus.Success,
                Items = res.Items?.Select(x => new CashflowItemResponse()
                {
                    UserId = x.UserId,
                    CashflowId = x.CashflowId,
                    Credit = x.Credit,
                    CreatedAt = x.CreatedAt,
                    CashflowTypeId = x.CashflowTypeId,
                    CurrencyId = x.CurrencyId,
                    CurrencyCode = x.CurrencyCode,
                    CashflowTypeName = x.CashflowTypeName,
                    CurrencyName = x.CurrencyName,
                    CreditTRY = x.CreditTRY,
                    CurrencyRate = x.CurrencyRate,
                    Debit = x.Debit,
                    DebitTRY = x.DebitTRY,
                    Description = x.Description,
                    DocumentNumber = x.DocumentNumber,
                    Status = ApiResponseStatus.Success
                })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new CashflowListResponse
            {
                Status = ApiResponseStatus.ServerError,
                Message = $"An error occurred while fetching cashflows. {ex.Message}"
            });
        }
    }
}
