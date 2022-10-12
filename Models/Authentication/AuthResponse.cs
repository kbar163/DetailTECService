namespace DetailTECService.Models
{
    //Modelo de respuesta a frontend para intento de login,
    //Si la autenticacion es exitosa, la propiedad authentication sera true,
    //de lo contrario, false.
    //La propiedad administrador identifica si el usuario que esta intentando
    //hacer login es un trabajador o un cliente. Si la propiedad es true, es
    //trabajador, de lo contrario es un cliente.

    
    public class AuthResponse
    {
        
        public bool logged { get; set; }
        public bool administrador {get; set; }

    }
    

}
