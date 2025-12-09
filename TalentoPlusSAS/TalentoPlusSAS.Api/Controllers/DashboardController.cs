using Microsoft.AspNetCore.Mvc;
using TalentoPlusSAS.Application.Services;

namespace TalentoPlusSAS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AiDashboardService _aiDashboardService;

        public DashboardController(AiDashboardService aiDashboardService)
        {
            _aiDashboardService = aiDashboardService;
        }

        [HttpGet("kpis")]
        public async Task<IActionResult> ObtenerKpis()
        {
            var kpis = await _aiDashboardService.ObtenerMetricasAsync();
            return Ok(kpis);
        }

        [HttpPost("consulta-ia")]
        public async Task<IActionResult> ConsultarIa([FromBody] ConsultaIaDto consulta)
        {
            if (string.IsNullOrWhiteSpace(consulta.Pregunta))
                return BadRequest("La pregunta no puede estar vacía.");

            var respuesta = await _aiDashboardService.ConsultarIaAsync(consulta.Pregunta);
            return Ok(new { respuesta });
        }
    }

    public class ConsultaIaDto
    {
        public string Pregunta { get; set; }
    }
}