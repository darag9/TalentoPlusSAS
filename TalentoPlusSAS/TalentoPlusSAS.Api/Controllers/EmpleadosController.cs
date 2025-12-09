using Microsoft.AspNetCore.Mvc;
using TalentoPlusSAS.Application.Services;
using TalentoPlusSAS.Domain.Entities;
using TalentoPlusSAS.Domain.Interfaces;

namespace TalentoPlusSAS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private readonly ExcelImportService _importService;
        private readonly CvGenerationService _cvService;
        private readonly IEmpleadoRepository _repository;

        public EmpleadosController(
            ExcelImportService importService, 
            CvGenerationService cvService,
            IEmpleadoRepository repository)
        {
            _importService = importService;
            _cvService = cvService;
            _repository = repository;
        }

        [HttpPost("importar")]
        public async Task<IActionResult> ImportarEmpleados(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Por favor sube un archivo válido.");

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    await _importService.ImportarEmpleadosAsync(stream);
                }
                
                return Ok(new { mensaje = "Importación completada exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al procesar el archivo: {ex.Message}");
            }
        }

        [HttpGet("descargar-cv/{documento}")]
        public async Task<IActionResult> DescargarCv(string documento)
        {
            try 
            {
                var pdfBytes = await _cvService.GenerarHojaDeVidaAsync(documento);
                return File(pdfBytes, "application/pdf", $"CV_{documento}.pdf");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Empleado no encontrado.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            return Ok("Controlador activo. Usa importar o descargar-cv.");
        }
    }
}