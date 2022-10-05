using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using DetailTECService.Models;
using DetailTECService.Data;

namespace TallerTECService.Controllers
{

    //ProductController hereda la clase ControllerBase, utilizada para el manejo
    //de endpoints.
    //ApiController identifica a la clase como un controlador en el framework.
    //ProductController se encarga de manejar operaciones CRUD para los productos registrados.
    //Route especifica la ruta para este controlador. En este caso local:
    //http://localhost:7163/api/manage/product
    [Route("api/manage/product")]
    [ApiController]
    [EnableCors("Policy")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        // GET api/manage/product/all
        [HttpGet("all")]
        public ActionResult<MultivalueProduct> GetAllProducts()
        {

            var response = _repository.GetAllProducts();
            return Ok(response);

        }

    }
}