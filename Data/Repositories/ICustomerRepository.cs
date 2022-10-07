using DetailTECService.Models;

//Esta interfaz dicta el contrato basico a seguir para cualquier
//implementacion del repositorio que maneja la logica de gestion de clientes de la solucion.
namespace DetailTECService.Data
{
    public interface ICustomerRepository
    {
        MultivalueCustomer GetAllCustomers();
        ActionResponse AddCustomer(Customer customer);
        ActionResponse DeleteCustomer(CustomerIdRequest customerId);
        ActionResponse ModifyCustomer(Customer customer);
        
    }
}
