namespace DetailTECService.Models
{
    public class WashType
    {
        public string nombre_lavado { get; set; }
        public int costo_personal { get; set; }
        public int precio { get; set; }
        public int duracion { get; set; }
        public int puntos_otorgados { get; set; }
        public int costo_puntos { get; set; }
        public List<string>? insumos { get; set; }
        public List<Role>? roles { get; set; }
    }
}