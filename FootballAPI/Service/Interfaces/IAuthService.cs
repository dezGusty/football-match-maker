namespace FootballAPI.Service.Interfaces
{
    public interface IAuthService
    {

        Task<bool> LoginAsync(HttpContext httpContext, string email, string password);
        Task LogoutAsync(HttpContext httpContext);
    }
}
