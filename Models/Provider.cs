namespace DetailTECService.Models
{
    //Modelo de datos de Provider
    //usado para la gestion de proveedores en la solucion.
    public class Provider
    {
        public string? cedula_juridica_proveedor { get; set; }
        public string? nombre { get; set; }
        public string? telefono { get; set; }
        public string? provincia { get; set; }
        public string? canton { get; set; }
        public string? distrito { get; set; }
        public string? correo_electronico{ get; set; }    
        
    }
}
