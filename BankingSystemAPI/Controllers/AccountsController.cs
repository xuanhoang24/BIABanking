using BankingSystemAPI.DataLayer;
using BankingSystemAPI.Models;
using BankingSystemAPI.Models.DTOs.Accounts;
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

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/accounts
        [HttpGet]
        public async Task<IActionResult> GetMyAccounts()
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")!
            );

            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId)
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
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")!
            );

            var account = new Account
            {
                UserId = userId,
                AccountName = dto.AccountName,
                AccountType = dto.AccountType,
                AccountNumber = await GenerateUniqueAccountNumberAsync(),
                Status = AccountStatus.Active,
                BalanceInCents = 0
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return Ok(new { account.Id });
        }

        // GET: api/accounts/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAccountDetail(int id)
        {
            var userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")!
            );

            var account = await _context.Accounts
                .Where(a => a.Id == id && a.UserId == userId)
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
    }
}