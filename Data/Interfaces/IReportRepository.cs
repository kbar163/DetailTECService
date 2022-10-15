using DetailTECService.Models;

//Esta interfaz dicta el contrato basico a seguir para cualquier
//implementacion del repositorio que maneja la logica de generacion de reportes.
namespace DetailTECService.Data
{
    public interface IReportRepository
    {
        ReportResponse GenerateReport(ReportRequest newReport);
    }
}