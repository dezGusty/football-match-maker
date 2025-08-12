using FootballAPI.Repository;
using FootballAPI.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using BCrypt.Net;

namespace FootballAPI.Service
{
    public class AuthService : IAuthService
    {


        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> LoginAsync(HttpContext httpContext, string email, string password)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return false;

            var claims = new List<Claim>
         {
             new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
             new Claim(ClaimTypes.Email, user.Email),
             new Claim(ClaimTypes.Role, user.Role.ToString()) 
         };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await httpContext.SignInAsync("Cookies", principal);
            return true;
        }


        public async Task LogoutAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync("Cookies");
        }
    }
}
