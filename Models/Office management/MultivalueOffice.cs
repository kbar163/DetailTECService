namespace DetailTECService.Models
{

    //Esta clase es usada como modelo de datos para respuesta de GET request
    //en el que se devuelve una lista de valores del modelo Office.
    public class MultivalueOffice
    {
        public bool exito { get; set; }
        public List<Office>? sucursales { get; set; }

    }
}
