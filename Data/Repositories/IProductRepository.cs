using DetailTECService.Models;

//Esta interfaz dicta el contrato basico a seguir para cualquier
//implementacion del repositorio que maneja la logica de manejo de productos de la solucion.
namespace DetailTECService.Data
{
    public interface IProductRepository
    {        
        MultivalueProduct GetAllProducts();
        ActionResponse AddProduct (Product newProduct);
        ActionResponse ModifyProduct (Product newProduct);
        ActionResponse DeleteProduct (ProductIdRequest deleteId);
    }
}