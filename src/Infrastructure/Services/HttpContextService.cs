using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    internal sealed class HttpContextService(IHttpContextAccessor contextAccessor) : IHttpContextService
    {
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

        public void SetCookie(string name, string value, int duration)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(duration),
                SameSite = SameSiteMode.None,
                Domain = "localhost",
            };

            _contextAccessor.HttpContext!.Response.Cookies.Append(name, value, cookieOptions);
        }
    }
}
