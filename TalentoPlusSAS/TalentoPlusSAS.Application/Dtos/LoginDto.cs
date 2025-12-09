namespace TalentoPlusSAS.Application.Dtos;

public class LoginDto
{
    public required string Documento  { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}