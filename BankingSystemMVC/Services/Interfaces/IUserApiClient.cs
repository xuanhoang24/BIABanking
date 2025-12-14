using BankingSystemMVC.Models.Users;

namespace BankingSystemMVC.Services.Interfaces
{
    public interface IUserApiClient
    {
        Task<UserMeViewModel?> GetMeAsync(string accessToken);
    }
}
