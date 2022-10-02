namespace DetailTECService.Models
{
    //Modelo de datos de Sucursal
    //usado para la gestion de sucursales en la solucion.
    public class Office
    {
        public string? nombre_sucursal { get; set; }
        public string? telefono { get; set; }
        public string? cedula_trabajador_gerente { get; set; }
        public string? provincia { get; set; }
        public string? canton { get; set; }
        public string? distrito { get; set; }
        public string? fecha_apertura { get; set; }
        public string? fecha_inicio_gerencia { get; set; }
        public string? nombre_trabajador_gerente { get; set; }
        public string? primer_apellido_trabajador_gerente { get; set; }
    
        
        
    }

}