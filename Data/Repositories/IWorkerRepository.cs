using DetailTECService.Models;

//Esta interfaz dicta el contrato basico a seguir para cualquier
//implementacion del repositorio que maneja la logica de manejo de trabajadores de la solucion.
namespace DetailTECService.Data
{
    public interface IWorkerRepository
    {
        MultivalueRole GetRoles();
        MultivaluePayment GetPaymentTypes();
        
    }
}
