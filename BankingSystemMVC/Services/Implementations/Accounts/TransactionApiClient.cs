using BankingSystemMVC.Models.ViewModels.Accounts.Transactions;
using BankingSystemMVC.Services.Interfaces.Accounts;
using System.Text.Json;

namespace BankingSystemMVC.Services.Implementations.Accounts
{
    public class TransactionApiClient : ITransactionApiClient
    {
        private readonly HttpClient _http;

        public TransactionApiClient(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("CustomerApi");
        }
        public async Task<(bool Success, string? Reference, string? ErrorMessage)> DepositAsync(DepositViewModel model)
        {
            var res = await _http.PostAsJsonAsync("api/transactions/deposit", model);
            
            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadFromJsonAsync<TransactionResultViewModel>();
                return (true, result!.Reference, null);
            }

            var errorMessage = await ExtractErrorMessageAsync(res);
            return (false, null, errorMessage);
        }

        public async Task<(bool Success, string? Reference, string? ErrorMessage)> WithdrawAsync(WithdrawViewModel model)
        {
            var res = await _http.PostAsJsonAsync("api/transactions/withdraw", model);
            
            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadFromJsonAsync<TransactionResultViewModel>();
                return (true, result!.Reference, null);
            }

            var errorMessage = await ExtractErrorMessageAsync(res);
            return (false, null, errorMessage);
        }

        public async Task<(bool Success, string? Reference, string? ErrorMessage)> TransferAsync(TransferViewModel model)
        {
            var res = await _http.PostAsJsonAsync("api/transactions/transfer", model);
            
            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadFromJsonAsync<TransactionResultViewModel>();
                return (true, result!.Reference, null);
            }

            var errorMessage = await ExtractErrorMessageAsync(res);
            return (false, null, errorMessage);
        }

        public async Task<(bool Success, string? Reference, string? ErrorMessage)> PaymentAsync(PaymentViewModel model)
        {
            var res = await _http.PostAsJsonAsync("api/transactions/payment", model);
            
            if (res.IsSuccessStatusCode)
            {
                var result = await res.Content.ReadFromJsonAsync<TransactionResultViewModel>();
                return (true, result!.Reference, null);
            }

            var errorMessage = await ExtractErrorMessageAsync(res);
            return (false, null, errorMessage);
        }

        private async Task<string> ExtractErrorMessageAsync(HttpResponseMessage response)
        {
            try
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var errorResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(errorBody, options);
                return errorResponse?.GetValueOrDefault("error") ?? "Transaction failed";
            }
            catch
            {
                return "Transaction failed";
            }
        }
    }
}
