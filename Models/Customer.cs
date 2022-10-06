namespace DetailTECService.Models
{
    //Modelo de datos de cliente
    //usado para la gestion de clientes en la solucion.
    public class Customer
    {
        public string? cedula_cliente { get; set; }
        public string? nombre { get; set; }
        public string? primer_apellido { get; set; }
        public string? segundo_apellido { get; set; }
        public string? correo_cliente { get; set; }
        public string? usuario { get; set; }
        public string? password_cliente { get; set; }
        public int puntos_acum { get; set; }
        public List<Address>? direcciones { get; set; }
        public List<string>? telefonos {get; set; }
    
        
        
    }

}
