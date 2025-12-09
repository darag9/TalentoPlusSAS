using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentoPlusSAS.Application.Dtos;
using TalentoPlusSAS.Application.Services;

namespace TalentoPlusSAS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto modelo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var (exito, mensaje) = await _authService.RegisterAsync(modelo);

        if (!exito)
        {
            return BadRequest(new { message = mensaje });
        }

        return Ok(new { message = mensaje });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto modelo)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var (exito, token, mensaje) = await _authService.LoginAsync(modelo);

        if (!exito)
        {
            return Unauthorized(new {message = mensaje});
        }
        
        return Ok(new { message = mensaje });
    }
}