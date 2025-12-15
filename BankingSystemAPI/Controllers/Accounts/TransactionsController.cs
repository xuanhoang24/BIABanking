using BankingSystemAPI.Models.DTOs.Accounts.Transactions;
using BankingSystemAPI.Services.Customer.Interfaces;
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

        private int GetCustomerId()
        {
            return int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")!
            );
        }

        // DEPOSIT
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(DepositRequestDto dto)
        {
            await _transactionService.DepositAsync(
                GetCustomerId(),
                dto.AccountId,
                dto.Amount,
                dto.Description
            );

            return Ok();
        }

        // WITHDRAW
        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(WithdrawRequestDto dto)
        {
            await _transactionService.WithdrawAsync(
                GetCustomerId(),
                dto.AccountId,
                dto.Amount,
                dto.Description
            );

            return Ok();
        }

        // TRANSFER
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(TransferRequestDto dto)
        {
            await _transactionService.TransferAsync(
                GetCustomerId(),
                dto.FromAccountId,
                dto.ToAccountNumber,
                dto.Amount,
                dto.Description
            );

            return Ok();
        }

        // PAYMENT
        [HttpPost("payment")]
        public async Task<IActionResult> Payment(PaymentRequestDto dto)
        {
            await _transactionService.PaymentAsync(
                GetCustomerId(),
                dto.AccountId,
                dto.Amount,
                dto.Merchant
            );

            return Ok();
        }
    }
}
