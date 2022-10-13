namespace DetailTECService.Models
{
    //Modelo de datos para respuesta a front-end despues de hacer cualquier
    //tipo de accion CRUD.
    public class BillResponse
    {
        public bool facturada { get; set; }
        public string? mensaje { get; set; }
    }
}