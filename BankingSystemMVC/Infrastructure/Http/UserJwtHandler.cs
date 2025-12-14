using System.Net.Http.Headers;

namespace BankingSystemMVC.Infrastructure.Http
{
    public class UserJwtHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserJwtHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor
                .HttpContext?
                .Request
                .Cookies["user_access_token"];

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
