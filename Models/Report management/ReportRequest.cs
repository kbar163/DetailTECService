namespace DetailTECService.Models
{
    //Modelo de datos para generacion de reportes
    public class ReportRequest
    {
        public int tipo_reporte { get; set; }
        public string? cedula_cliente { get; set; }
    }
}