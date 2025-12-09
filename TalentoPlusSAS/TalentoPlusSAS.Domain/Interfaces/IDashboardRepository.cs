namespace TalentoPlusSAS.Domain.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardKpis> GetKpisAsync();

        Task<List<Dictionary<string, object>>> EjecutarConsultaDinamicaAsync(string sql);
    }

    public class DashboardKpis
    {
        public int TotalEmpleados { get; set; }
        public int EmpleadosEnVacaciones { get; set; }
    }
}