using ExcelDataReader;
using System.Globalization;
using System.Text;
using TalentoPlusSAS.Domain.Entities;
using TalentoPlusSAS.Domain.Interfaces;

namespace TalentoPlusSAS.Application.Services
{
    public class ExcelImportService
    {
        private readonly IEmpleadoRepository _repository;

        public ExcelImportService(IEmpleadoRepository repository)
        {
            _repository = repository;
        }

        public async Task ImportarEmpleadosAsync(Stream fileStream)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Console.WriteLine("--- Iniciando lectura UTC del archivo ---");

            IExcelDataReader reader;

            try 
            {
                reader = ExcelReaderFactory.CreateReader(fileStream, new ExcelReaderConfiguration()
                {
                    FallbackEncoding = Encoding.GetEncoding(1252)
                });
            }
            catch (ExcelDataReader.Exceptions.HeaderException)
            {
                if (fileStream.CanSeek) fileStream.Position = 0;
                reader = ExcelReaderFactory.CreateCsvReader(fileStream, new ExcelReaderConfiguration()
                {
                    FallbackEncoding = Encoding.GetEncoding(1252)
                });
            }

            using (reader)
            {
                reader.Read(); // Leer encabezados
                
                var mapaColumnas = new Dictionary<string, int>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var nombreColumna = reader.GetValue(i)?.ToString()?.Trim().ToLower();
                    if (!string.IsNullOrEmpty(nombreColumna) && !mapaColumnas.ContainsKey(nombreColumna))
                    {
                        mapaColumnas.Add(nombreColumna, i);
                    }
                }

                string Get(string colName)
                {
                    colName = colName.ToLower();
                    if (mapaColumnas.ContainsKey(colName))
                    {
                        var index = mapaColumnas[colName];
                        return reader.GetValue(index)?.ToString()?.Trim() ?? "";
                    }
                    return "";
                }

                var empleados = new List<Empleado>();
                int filaActual = 1;

                while (reader.Read())
                {
                    filaActual++;
                    try
                    {
                        if (string.IsNullOrWhiteSpace(Get("Documento"))) continue;

                        var empleado = new Empleado
                        {
                            Documento = Get("Documento"),
                            Nombres = Get("Nombres"),
                            Apellidos = Get("Apellidos"),
                            Cargo = Get("Cargo"),
                            Estado = string.IsNullOrEmpty(Get("Estado")) ? "Activo" : Get("Estado"),
                            NivelEducativo = Get("NivelEducativo"),
                            Departamento = Get("Departamento"),
                            Email = Get("Email"),
                            Telefono = Get("Telefono"),
                            Direccion = Get("Direccion"),
                            PerfilProfesional = Get("PerfilProfesional"),

                            Salario = ParsearDecimal(Get("Salario")),
                            
                            // CORRECCIÓN 1: Convertir fecha leída a UTC
                            FechaIngreso = ParsearFecha(Get("FechaIngreso")),
                            
                            // CORRECCIÓN 2: Fecha de nacimiento forzada a UTC
                            FechaNacimiento = DateTime.SpecifyKind(new DateTime(1990, 1, 1), DateTimeKind.Utc)
                        };

                        empleados.Add(empleado);
                        Console.WriteLine($"Fila {filaActual}: Procesado {empleado.Nombres}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ERROR en fila {filaActual}: {ex.Message}");
                    }
                }

                Console.WriteLine($"--- Guardando {empleados.Count} empleados ---");
                foreach (var emp in empleados)
                {
                    var existente = await _repository.GetByDocumentoAsync(emp.Documento);
                    if (existente == null)
                    {
                        await _repository.AddAsync(emp);
                    }
                }
                Console.WriteLine("--- Importación Finalizada ---");
            }
        }

        private decimal ParsearDecimal(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return 0;
            valor = valor.Replace("$", "").Replace(" ", "").Trim();
            if (decimal.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal resultado))
                return resultado;
            return 0;
        }

        private DateTime ParsearFecha(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return DateTime.UtcNow; // UTC

            string[] formatos = { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "yyyy/MM/dd" };
            
            if (DateTime.TryParseExact(valor, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime resultado))
            {
                // CORRECCIÓN 3: ¡Esto es lo que arregla el error de PostgreSQL!
                return DateTime.SpecifyKind(resultado, DateTimeKind.Utc);
            }
            return DateTime.UtcNow; // UTC
        }
    }
}