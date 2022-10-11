namespace DetailTECService.Models
{
    //Modelo de datos que representa una cita de lavado,
    public class NewAppRequest
    {
        public string? cedula_cliente { get; set; }
        public string? placa_vehiculo { get; set; }
        public string?  nombre_sucursal { get; set; }
        public string? nombre_lavado { get; set; }
        public string? hora { get; set; }
        public int facturada { get; set; }
        // public int duracion { get; set; }
    }
    

}