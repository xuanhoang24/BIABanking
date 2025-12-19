using BankingSystemAPI.Extensions;
using BankingSystemAPI.Application.Dtos.Transactions;
using BankingSystemAPI.Application.Services.Interfaces.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            var reference = await _transactionService.DepositAsync(
                User.GetRequiredUserId(),
                dto.AccountId,
                dto.Amount,
                dto.Description
            );

            return Ok(new TransactionResultDto { Reference = reference });
        }

        // WITHDRAW
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(WithdrawRequestDto dto)
        {
            var reference = await _transactionService.WithdrawAsync(
                User.GetRequiredUserId(), 
                dto.AccountId,
                dto.Amount,
                dto.Description
            );

            return Ok(new TransactionResultDto { Reference = reference });
        }

        // TRANSFER
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(TransferRequestDto dto)
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

        // PAYMENT
        [HttpPost("payment")]
        public async Task<IActionResult> Payment(PaymentRequestDto dto)
        {
            var reference = await _transactionService.PaymentAsync(
                User.GetRequiredUserId(), 
                dto.AccountId,
                dto.Amount,
                dto.Merchant
            );

            return Ok(new TransactionResultDto { Reference = reference });
        }
    }
}