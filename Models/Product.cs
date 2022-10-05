namespace DetailTECService.Models
{
    //Modelo de datos de Producto
    //usado para la gestion de productos en la solucion.
    public class Product
    {
        public string? nombre_insumo { get; set; }
        public int? costo { get; set; }
        public string? marca { get; set; }
        public string? cedula_juridica_proveedor { get; set; }
        public string? nombre_proveedor{get; set; }
    }
}