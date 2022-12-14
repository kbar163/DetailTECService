namespace DetailTECService.Models
{
    //Modelo de datos que representa una cita de lavado,
    public class Appointment
    {
        
        public int id_cita { get; set; }
        public string? cedula_cliente { get; set; }
        public string? nombre_cliente{ get; set; }
        public string? apellido_cliente { get; set; }
        public string? placa_vehiculo { get; set; }
        public string?  nombre_sucursal { get; set; }
        public string? nombre_lavado { get; set; }
        public string? cedula_trabajador { get; set; }
        public string? nombre_trabajador { get; set; }
        public string? apellido_trabajador { get; set; }
        public string? hora { get; set; }
        public bool facturada { get; set; }
        public int duracion { get; set; }
    }
    

}
