namespace DetailTECService.Models
{
    //Modelo de datos que representa una direccion con provincia canton y distrito,
    //usado por el modelo de datos de Customer para mantener los datos de las posibles
    //direcciones de cada cliente.

    
    public class Address
    {
        
        public string? provincia { get; set; }
        public string? canton {get; set; }
        public string? distrito {get; set; }

    }
    

}