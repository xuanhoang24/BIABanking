using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models;
using BankingSystemAPI.Models.DTOs.Accounts;
using BankingSystemAPI.Models.Users;
using BankingSystemAPI.Models.Users.Admin;
using BankingSystemAPI.Services.Admin.Implements;
using BankingSystemAPI.Services.Customer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace BankingSystemAPI.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuditService _auditService;
        private readonly IAccountService _accountService;

        public AccountsController(AppDbContext context, AuditService auditService, IAccountService accountService)
        {
            _context = context;
            _auditService = auditService;
            _accountService = accountService;
        }

        // GET: api/accounts
        [HttpGet]
        public async Task<IActionResult> GetMyAccounts()
        {
            var customerId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")!
            );

            var accounts = await _context.Accounts
                .Where(a => a.CustomerId == customerId)
                .Select(a => new AccountSummaryDto
                {
                    Id = a.Id,
                    AccountNumber = a.AccountNumber,
                    AccountName = a.AccountName,
                    AccountType = a.AccountType.ToString(),
                    Balance = a.Balance,
                    Status = a.Status.ToString()
                })
                .ToListAsync();

            return Ok(accounts);
        }

        // POST: api/accounts
        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccountRequestDto dto)
        {
            var customerId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")!
            );

            var account = new Account
            {
                CustomerId = customerId,
                AccountName = dto.AccountName,
                AccountType = dto.AccountType,
                AccountNumber = await GenerateUniqueAccountNumberAsync(),
                Status = AccountStatus.Active,
                BalanceInCents = 0
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                AuditAction.AccountCreated,
                "Account",
                account.Id,
                customerId,
                $"Account created with number {account.AccountNumber}"
            );

            return Ok(new { account.Id });
        }

        // GET: api/accounts/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAccountDetail(int id)
        {
            var customerId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")!
            );

            var account = await _context.Accounts
                .Where(a => a.Id == id && a.CustomerId == customerId)
                .Select(a => new AccountDetailDto
                {
                    Id = a.Id,
                    AccountNumber = a.AccountNumber,
                    AccountName = a.AccountName,
                    AccountType = a.AccountType.ToString(),
                    Balance = a.Balance,
                    Status = a.Status.ToString()
                })
                .FirstOrDefaultAsync();

            if (account == null)
                return NotFound();

            return Ok(account);
        }

        // Helpers
        private async Task<string> GenerateUniqueAccountNumberAsync()
        {
            string accountNumber;

            do
            {
                var bytes = RandomNumberGenerator.GetBytes(8);
                var number = Math.Abs(BitConverter.ToInt64(bytes, 0));
                accountNumber = ((number % 90000000000L) + 10000000000L).ToString();
            }
            while (await _context.Accounts.AnyAsync(a => a.AccountNumber == accountNumber));

            return accountNumber;
        }

        // POST: api/accounts/{id}/deposit
        [HttpPost("{id:int}/deposit")]
        public async Task<IActionResult> Deposit(int id, DepositRequestDto dto)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")!
            );

            try
            {
                var account = await _accountService.DepositAsync(
                    id,
                    userId,
                    dto.AmountInCents,
                    dto.Description
                );

                if (account == null)
                    return NotFound();

                return Ok(new
                {
                    account.Id,
                    account.AccountNumber,
                    NewBalance = account.Balance
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}