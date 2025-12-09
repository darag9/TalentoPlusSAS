using Microsoft.EntityFrameworkCore;
using TalentoPlusSAS.Domain.Entities;
using TalentoPlusSAS.Domain.Interfaces;
using TalentoPlusSAS.Infrastructure.Data;

namespace TalentoPlusSAS.Infrastructure.Repositories;

public class EmpleadoRepository : IEmpleadoRepository
{
    private readonly ApplicationDbContext _context;

    public EmpleadoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Empleado?> GetByDocumentoAsync(string documento)
    {
        return await _context.Empleados
            .FirstOrDefaultAsync(e => e.Documento == documento);
    }

    public async Task AddAsync(Empleado empleado)
    {
        await _context.Empleados.AddAsync(empleado);
        await _context.SaveChangesAsync();
    }

    public Task UpdateAsync(Empleado empleado)
    {
        _context.Empleados.Update(empleado);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}