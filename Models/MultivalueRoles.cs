namespace DetailTECService.Models
{

    //Esta clase es usada como modelo de datos para respuesta de GET request
    //en el que se devuelve una lista de valores del modelo Role.
    public class MultivalueRole
    {
        public bool exito { get; set; }
        public List<Role>? roles { get; set; }

    }
}
