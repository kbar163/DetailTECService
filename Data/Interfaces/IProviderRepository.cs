using DetailTECService.Models;

//Esta interfaz dicta el contrato basico a seguir para cualquier
//implementacion del repositorio que maneja la logica de manejo de proveedores de la solucion.
namespace DetailTECService.Data
{
    public interface IProviderRepository
    {        
        MultivalueProvider GetAllProviders();
        ActionResponse AddProvider (Provider newProvider);
        ActionResponse ModifyProvider (Provider newProvider);
        ActionResponse DeleteProvider (ProviderIdRequest deleteId);
    }
}