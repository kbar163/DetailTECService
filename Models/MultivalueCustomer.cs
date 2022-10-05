namespace DetailTECService.Models
{

    //Esta clase es usada como modelo de datos para respuesta de GET request
    //en el que se devuelve una lista de valores del modelo Customer.
    public class MultivalueCustomer
    {
        public bool exito { get; set; }
        public List<Customer>? clientes { get; set; }

    }
}