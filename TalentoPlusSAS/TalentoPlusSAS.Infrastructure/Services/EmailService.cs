using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using TalentoPlusSAS.Domain.Interfaces;

namespace TalentoPlusSAS.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task SendEmailAsync(string emailDestino, string asunto, string mensajeHtml)
    {
        var smtpHost = _configuration["Smt:Host"];
        var smtpPort = int.Parse(_configuration["smtp:Port"] ?? "587");
        var smtpUser = _configuration["smtp:User"];
        var smtpPass = _configuration["smtp:Password"];

        using (var client = new SmtpClient(smtpHost, smtpPort))
        {
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(smtpUser, smtpPass);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser!),
                Subject = asunto,
                Body = mensajeHtml,
                IsBodyHtml = true
            };
            mailMessage.To.Add(emailDestino);
            
            await client.SendMailAsync(mailMessage);
        }
    }
}