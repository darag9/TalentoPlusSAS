using System.Reflection.Metadata;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TalentoPlusSAS.Domain.Interfaces;

namespace TalentoPlusSAS.Application.Services;

public class CvGenerationService
{
    private readonly IEmpleadoRepository _repository;

    public CvGenerationService(IEmpleadoRepository repository)
    {
        _repository = repository;
    }

    public async Task<byte[]> GenerarHojaDeVidaAsync(string documento)
    {
        var empleado = await _repository.GetByDocumentoAsync(documento);

        if (empleado == null)
            throw new Exception("Empleado no encontrado");

        return QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text($"{empleado.Nombres} {empleado.Apellidos}")
                                .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                            column.Item().Text(empleado.Cargo).FontSize(14);
                        });
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(col =>
                    {
                        col.Item().Text("Datos Personales y de Contacto").Bold().FontSize(14).Underline();
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100);
                                columns.RelativeColumn();
                            });

                            table.Cell().Text("Documento:");
                            table.Cell().Text(empleado.Documento);
                            table.Cell().Text("Email:");
                            table.Cell().Text(empleado.Email);
                            table.Cell().Text("Teléfono:");
                            table.Cell().Text(empleado.Telefono);
                            table.Cell().Text("Dirección:");
                            table.Cell().Text(empleado.Direccion);
                            table.Cell().Text("Nacimiento:");
                            table.Cell().Text(empleado.FechaNacimiento.ToShortDateString());
                        });

                        col.Item().PaddingTop(20).Text("Información Laboral").Bold().FontSize(14).Underline();
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100);
                                columns.RelativeColumn();
                            });

                            table.Cell().Text("Departamento:");
                            table.Cell().Text(empleado.Departamento);
                            table.Cell().Text("Fecha Ingreso:");
                            table.Cell().Text(empleado.FechaIngreso.ToShortDateString());
                            table.Cell().Text("Estado:");
                            table.Cell().Text(empleado.Estado);
                        });

                        col.Item().PaddingTop(20).Text("Perfil Profesional").Bold().FontSize(14).Underline();
                        col.Item().PaddingTop(5).Text($"Nivel Educativo: {empleado.NivelEducativo}").SemiBold();
                        col.Item().PaddingTop(5).Text(empleado.PerfilProfesional);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generado por TalentoPlusSAS - ");
                        x.CurrentPageNumber();
                    });
            });
        }).GeneratePdf();
    }
}
 