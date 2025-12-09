namespace TalentoPlusSAS.Domain.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string emailDestino, string asunto, string mensajeHtml);
}