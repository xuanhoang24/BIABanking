using BankingSystemMVC.Models.Accounts.Transactions;
using BankingSystemMVC.Services.Interfaces;

namespace BankingSystemMVC.Services.Implements
{
    public class TransactionApiClient : ITransactionApiClient
    {
        private readonly HttpClient _http;

        public TransactionApiClient(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("CustomerApi");
        }

        public async Task<bool> DepositAsync(DepositViewModel model)
        {
            var res = await _http.PostAsJsonAsync("api/transactions/deposit", model);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> WithdrawAsync(WithdrawViewModel model)
        {
            var res = await _http.PostAsJsonAsync("api/transactions/withdraw", model);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> TransferAsync(TransferViewModel model)
        {
            var res = await _http.PostAsJsonAsync("api/transactions/transfer", model);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> PaymentAsync(PaymentViewModel model)
        {
            var res = await _http.PostAsJsonAsync("api/transactions/payment", model);
            return res.IsSuccessStatusCode;
        }
    }
}
