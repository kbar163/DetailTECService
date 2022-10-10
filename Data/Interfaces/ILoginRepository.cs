using DetailTECService.Models;

//Esta interfaz dicta el contrato basico a seguir para cualquier
//implementacion del repositorio que maneja la logica de login de la solucion.
namespace DetailTECService.Data
{
    public interface ILoginRepository
    {
        AuthResponse AuthCheck(LoginData userData);
        
    }
}
