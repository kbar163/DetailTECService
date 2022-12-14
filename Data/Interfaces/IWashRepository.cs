using DetailTECService.Models;

//Esta interfaz dicta el contrato basico a seguir para cualquier
//implementacion del repositorio que maneja la logica de gestion de tipos de lavado de la solucion.
namespace DetailTECService.Data
{
    public interface IWashRepository
    {
        MultivalueWash GetAllWashTypes();
        ActionResponse AddWashType(WashType newWash);
        ActionResponse DeleteWashType(WashIdRequest deleteName);
        ActionResponse ModifyWashType(WashType newWash);
    }
}