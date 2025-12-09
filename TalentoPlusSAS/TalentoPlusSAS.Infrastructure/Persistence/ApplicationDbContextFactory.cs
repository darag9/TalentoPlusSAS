using Microsoft.EntityFrameworkCore.Design;
using TalentoPlusSAS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TalentoPlusSAS.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "TalentoPlusSAS.API");

        // Si el comando se ejecuta desde dentro de la carpeta API, ajustamos el path
        if (!Directory.Exists(basePath))
        {
            basePath = Directory.GetCurrentDirectory();
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables() // Para leer variables de entorno si las hubiera
            .Build();

        // 2. Construir las opciones del DbContext
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Importante: Le decimos que las migraciones se guardan en el proyecto de Infrastructure
        builder.UseNpgsql(connectionString, b => b.MigrationsAssembly("TalentoPlusSAS.Infrastructure"));

        return new ApplicationDbContext(builder.Options);
    }
}