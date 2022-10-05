using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using DetailTECService.Data;
using DetailTECService.Models;

namespace TallerTECService.Controllers
{
    //LoginController hereda la clase ControllerBase, utilizada para el manejo
    //del endpoints.
    //ApiController identifica a la clase como un controlador en el framework.
    //LoginController se encarga de manejar el endpoint que permite a los usuarios hacer login.
    //Route especifica la ruta para este controlador. En este caso local:
    //http://localhost:7163/api/login

    [Route("api/login")]
    [ApiController]
    [EnableCors("Policy")]
    public class CustomerController : ControllerBase
    {
        private readonly ILoginRepository _repository;

        public CustomerController(ILoginRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("all")]
        ActionResult<MultivalueCustomer> GetAllCustomers()
        {
            throw new NotImplementedException();
        }




    }

}
