using BankingSystemAPI.Configuration;
using BankingSystemAPI.Application.Dtos.Transactions;
using BankingSystemAPI.Application.Services.Interfaces.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.Controllers.Accounts
{
    [ApiController]
    [Route("api/transactions")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // DEPOSIT
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(DepositRequestDto dto)
        {
            try
            {
                var reference = await _transactionService.DepositAsync(
                    User.GetRequiredUserId(),
                    dto.AccountId,
                    dto.Amount,
                    dto.Description
                );

                return Ok(new TransactionResultDto { Reference = reference });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        // WITHDRAW
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(WithdrawRequestDto dto)
        {
            try
            {
                var reference = await _transactionService.WithdrawAsync(
                    User.GetRequiredUserId(), 
                    dto.AccountId,
                    dto.Amount,
                    dto.Description
                );

                return Ok(new TransactionResultDto { Reference = reference });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        // TRANSFER
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(TransferRequestDto dto)
        {
            try
            {
                var reference = await _transactionService.TransferAsync(
                    User.GetRequiredUserId(), 
                    dto.FromAccountId,
                    dto.ToAccountNumber,
                    dto.Amount,
                    dto.Description
                );

                return Ok(new TransactionResultDto { Reference = reference });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        // PAYMENT
        [HttpPost("payment")]
        public async Task<IActionResult> Payment(PaymentRequestDto dto)
        {
            try
            {
                var reference = await _transactionService.PaymentAsync(
                    User.GetRequiredUserId(), 
                    dto.AccountId,
                    dto.Amount,
                    dto.Merchant
                );

                return Ok(new TransactionResultDto { Reference = reference });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
}