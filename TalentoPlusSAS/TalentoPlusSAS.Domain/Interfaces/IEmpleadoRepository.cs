using TalentoPlusSAS.Domain.Entities;

namespace TalentoPlusSAS.Domain.Interfaces;

public interface IEmpleadoRepository
{
    Task<Empleado?> GetByDocumentoAsync(string documento);
    Task AddAsync(Empleado empleado);
    Task UpdateAsync(Empleado empleado);
    Task SaveChangesAsync();
}