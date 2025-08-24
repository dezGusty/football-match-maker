using FootballAPI.Models.Enums;
﻿namespace FootballAPI.Service.Interfaces
{
    public interface IAuthService
    {

        Task<string?> LoginAsync( string email, string password);
        Task LogoutAsync(HttpContext httpContext);
    }
}
