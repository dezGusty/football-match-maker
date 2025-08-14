using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballAPI.Service.Interfaces
{
    public interface IEmailService
    {
        Task SendSetPasswordEmailAsync(string email, string username, string setPasswordUrl);
    }
}