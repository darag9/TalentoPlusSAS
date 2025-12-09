using System.Data;
using Microsoft.EntityFrameworkCore;
using TalentoPlusSAS.Domain.Interfaces;
using TalentoPlusSAS.Infrastructure.Data;

namespace TalentoPlusSAS.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<DashboardKpis> GetKpisAsync()
        {
            var total = await _context.Empleados.CountAsync();
            var vacaciones = await _context.Empleados.CountAsync(e => e.Estado == "Vacaciones");

            return new DashboardKpis
            {
                TotalEmpleados = total,
                EmpleadosEnVacaciones = vacaciones
            };
        }

        public async Task<List<Dictionary<string, object>>> EjecutarConsultaDinamicaAsync(string sql)
        {
            var resultados = new List<Dictionary<string, object>>();
            var connection = _context.Database.GetDbConnection();

            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var fila = new Dictionary<string, object>();
                            
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var nombreColumna = reader.GetName(i);
                                var valor = reader.GetValue(i);
                                fila[nombreColumna] = valor;
                            }
                            resultados.Add(fila);
                        }
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            return resultados;
        }
}