namespace DetailTECService.Models
{
    //Modelo de datos que representa una entrada en la 
    //tabla TIPO_PAGO en la base de datos.
    public class Payment
    {
        public int id_tipo_pago { get; set; }
        public string? tipo_pago { get; set; }
    }
}
