namespace DetailTECService.Models
{
    //Modelo de datos utilizado para enviar la lista de 
    // todas las citas registradas del lavado
    public class MultivalueAppointment
    {
        
        public bool exito { get; set; }
        public List<Appointment>? citas { get; set; }
    }
    

}