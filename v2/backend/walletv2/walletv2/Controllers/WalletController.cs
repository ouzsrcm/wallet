using Microsoft.AspNetCore.Mvc;
using walletv2.Data.Entities.DataTransferObjects;
using walletv2.Data.Services;
using walletv2.Dtos;

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
            var incomeExpenseId = await _walletService.CreateIncomeExpenseAsync(new CreateIncomeExpenseDto()
            {
                UserId = request.UserId,
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

}
