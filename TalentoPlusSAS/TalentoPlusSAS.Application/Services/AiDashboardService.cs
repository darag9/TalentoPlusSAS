using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using TalentoPlusSAS.Domain.Interfaces;

namespace TalentoPlusSAS.Application.Services
{
    public class AiDashboardService
    {
        private readonly IDashboardRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AiDashboardService(IDashboardRepository repository, IConfiguration configuration, HttpClient httpClient)
        {
            _repository = repository;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<DashboardKpis> ObtenerMetricasAsync()
        {
            return await _repository.GetKpisAsync();
        }

        public async Task<string> ConsultarIaAsync(string preguntaUsuario)
        {
            // 1. Construir el Prompt con el contexto de TU base de datos
            var systemPrompt = @"
                Eres un experto en SQL PostgreSQL. Tienes una tabla llamada 'Empleados' con estas columnas:
                Documento (text), Nombres (text), Apellidos (text), Cargo (text), Salario (decimal), 
                FechaIngreso (date), Estado (text), NivelEducativo (text), Departamento (text).
                
                Reglas:
                1. Responde ÚNICAMENTE con la consulta SQL. Nada de texto extra ni markdown.
                2. Si la pregunta es sobre contar, usa SELECT COUNT(*).
                3. El estado 'Vacaciones' se escribe tal cual.
                4. Usa ILIKE para búsquedas de texto flexible.
            ";

            // 2. Llamar a OpenAI para obtener el SQL
            var apiKey = _configuration["OpenAi:ApiKey"];
            if (string.IsNullOrEmpty(apiKey)) return "Error: API Key de IA no configurada.";

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = preguntaUsuario }
                },
                temperature = 0
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
            
            if (!response.IsSuccessStatusCode) return "Error al consultar el servicio de IA.";

            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            var sqlGenerado = jsonResponse.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            if (string.IsNullOrEmpty(sqlGenerado) || 
                sqlGenerado.Contains("DELETE", StringComparison.OrdinalIgnoreCase) || 
                sqlGenerado.Contains("DROP", StringComparison.OrdinalIgnoreCase) ||
                sqlGenerado.Contains("UPDATE", StringComparison.OrdinalIgnoreCase))
            {
                return "Lo siento, solo puedo realizar consultas de lectura por seguridad.";
            }

            try
            {
                var resultados = await _repository.EjecutarConsultaDinamicaAsync(sqlGenerado);

                if (resultados.Count == 0) return "No encontré registros que coincidan con tu consulta.";
                
                if (resultados.Count == 1 && resultados[0].Count == 1)
                {
                    return $"El resultado es: {resultados[0].Values.First()}";
                }

                return $"Encontré {resultados.Count} registros que coinciden. (Se muestra el primero: {string.Join(", ", resultados[0].Values)})";
            }
            catch (Exception ex)
            {
                return $"Error ejecutando la consulta generada: {ex.Message}";
            }
        }
    }
}