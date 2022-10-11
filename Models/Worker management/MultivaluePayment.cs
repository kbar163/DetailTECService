namespace DetailTECService.Models
{

    //Esta clase es usada como modelo de datos para respuesta de GET request
    //en el que se devuelve una lista de valores del modelo Payment.
    public class MultivaluePayment
    {
        public bool exito { get; set; }
        public List<Payment>? tipos_pago { get; set; }

    }
}
