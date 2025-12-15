
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
        public async Task<string> DepositAsync(DepositViewModel model)
        {
            var res = await _http.PostAsJsonAsync("api/transactions/deposit", model);
            res.EnsureSuccessStatusCode();

            var result = await res.Content.ReadFromJsonAsync<TransactionResultViewModel>();
            return result!.Reference;
        }

        public async Task<string> WithdrawAsync(WithdrawViewModel model)
        {
            var res = await _http.PostAsJsonAsync("api/transactions/withdraw", model);
            res.EnsureSuccessStatusCode();

            var result = await res.Content.ReadFromJsonAsync<TransactionResultViewModel>();
            return result!.Reference;
        }

        public async Task<string> TransferAsync(TransferViewModel model)
        {
            var res = await _http.PostAsJsonAsync("api/transactions/transfer", model);
            res.EnsureSuccessStatusCode();

            var result = await res.Content.ReadFromJsonAsync<TransactionResultViewModel>();
            return result!.Reference;
        }

        public async Task<string> PaymentAsync(PaymentViewModel model)
        {
            var res = await _http.PostAsJsonAsync("api/transactions/payment", model);
            res.EnsureSuccessStatusCode();

            var result = await res.Content.ReadFromJsonAsync<TransactionResultViewModel>();
            return result!.Reference;
        }
    }
}
