namespace DetailTECService.Models
{
    //Modelo de datos de Trabajador
    //usado para la gestion de trabajadores en la solucion.
    public class Worker
    {
        public string cedula_trabajador { get; set; }
        public DateOnly fecha_nacimiento { get; set; }
        public string nombre { get; set; }
        public string primer_apellido { get; set; }
        public string segundo_apellido { get; set; }
        public int id_rol { get; set; }
        public int id_tipo_pago { get; set; }
        public DateOnly fecha_ingreso { get; set; }
        public string? password_trabajador { get; set; }
    
        
        
    }

}
