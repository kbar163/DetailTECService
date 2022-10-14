namespace DetailTECService.Models
{
    //Modelo de datos que representa una una peticion de factura para una cita.

    public class BillRequest
    {
        
        public int id_cita { get; set; }
        public int cantidad_bebidas {get; set; }
        public int cantidad_snacks {get; set; }
        public int pago_puntos {get; set; }

    }
    

}
