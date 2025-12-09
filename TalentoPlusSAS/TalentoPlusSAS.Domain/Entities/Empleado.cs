using System.ComponentModel.DataAnnotations;

namespace TalentoPlusSAS.Domain.Entities;

public class Empleado
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Documento { get; set; } = string.Empty;
    
    [Required]
    public string Nombres { get; set; } = string.Empty;
    
    [Required]
    public string Apellidos { get; set; } = string.Empty;
    
    [DataType(DataType.Date)]
    public DateTime FechaNacimiento { get; set; }
    
    public string Direccion { get; set; } = string.Empty;
    
    public string Telefono { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Cargo { get; set; } = string.Empty;
    
    public decimal Salario { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime FechaIngreso { get; set; }
    
    [Required]
    public string Estado { get; set; } = string.Empty;
    
    [Required]
    public string NivelEducativo { get; set; } = string.Empty;
    
    public string PerfilProfesional { get; set; } = string.Empty;
    
    [Required]
    public string Departamento { get; set; } = string.Empty;

}