namespace DetailTECService.Models
{

    //Esta clase es usada como modelo de datos para respuesta de GET request
    //en el que se devuelve una lista de valores del modelo Worker.
    public class MultivalueWorker
    {
        public bool exito { get; set; }
        public List<Worker>? trabajadores { get; set; }

    }
}