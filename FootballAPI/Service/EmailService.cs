using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using FootballAPI.Service.Interfaces;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    


}


