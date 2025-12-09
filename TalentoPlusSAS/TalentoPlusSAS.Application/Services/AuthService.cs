using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TalentoPlusSAS.Application.Dtos;
using TalentoPlusSAS.Domain.Interfaces;
namespace TalentoPlusSAS.Application.Services;

public class AuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(UserManager<IdentityUser> userManager, IEmpleadoRepository empleadoRepository, IConfiguration configuration,  IEmailService emailService)
    {
        _userManager = userManager;
        _empleadoRepository = empleadoRepository;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<(bool Exito, string Mensaje)> RegisterAsync(RegisterDto modelo)
    {
        var empleado = await _empleadoRepository.GetByDocumentoAsync(modelo.Documento);
        if (empleado == null)
        {
            return (false, "El documento no corresponde a ningun empleado registrado en la nomina.");
        }

        if (!empleado.Email.Equals(modelo.Email, StringComparison.OrdinalIgnoreCase))
        {
            return (false, "El correo proporcionado no coincide con el registrado en la empresa.");
        }
        
        var user = new IdentityUser
        {
            UserName = modelo.Email,
            Email = modelo.Email,
            PhoneNumber = modelo.Documento 
        };

        var result = await _userManager.CreateAsync(user, modelo.Password);

        if (!result.Succeeded)
        {
            var errores = string.Join(", ", result.Errors.Select(e => e.Description));
            return (false, $"Error al crear usuario: {errores}");
        }

        string asunto = "Bienvenido a TalentoPlusSAS";
        string cuerpo = $@"
                <h1>Hola, {empleado.Nombres}</h1>
                <p>Tu registro en la plataforma de Recursos Humanos ha sido exitoso.</p>
                <p>Ya puedes iniciar sesión y descargar tu hoja de vida.</p>";

        try 
        {
            await _emailService.SendEmailAsync(modelo.Email, asunto, cuerpo);
        }
        catch 
        {
            // Loguear error de email, pero el registro fue exitoso
        }
        

        return (true, "Registro exitoso. Ya puede iniciar sesion.");
    }
    
    public async Task<(bool Exito, string Token, string Mensaje)> LoginAsync(LoginDto modelo)
    {
        var user = await _userManager.FindByEmailAsync(modelo.Email);
            
        if (user == null || !await _userManager.CheckPasswordAsync(user, modelo.Password))
        {
            return (false, string.Empty, "Credenciales inválidas.");
        }

        if (user.PhoneNumber != modelo.Documento) 
        {
            return (false, string.Empty, "El documento no coincide con el usuario registrado.");
        }

        var token = GenerarJwtToken(user);
        return (true, token, "Login exitoso.");
    }
    
    private string GenerarJwtToken(IdentityUser user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("Documento", user.PhoneNumber!) // Guardamos el documento en el token
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2), // Token válido por 2 horas
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}