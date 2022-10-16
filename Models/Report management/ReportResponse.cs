namespace DetailTECService.Models
{
    //Modelo de datos para respuesta a front-end al hacer un request de reporte
    public class ReportResponse
    {
        public bool generado { get; set; }
        public string? mensaje { get; set; }
    }
}
