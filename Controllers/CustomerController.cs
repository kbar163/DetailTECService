using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using DetailTECService.Models;
using DetailTECService.Data;

namespace DetailTECService.Controllers
{

    //CustomerController hereda la clase ControllerBase, utilizada para el manejo
    //del endpoints.
    //ApiController identifica a la clase como un controlador en el framework.
    //CustomerController Se encarga de manejar operaciones CRUD para los clientes registrados.
    //Route especifica la ruta para este controlador. En este caso local:
    //http://localhost:7163/api/manage/customer
    [Route("api/manage/customer")]
    [ApiController]
    [EnableCors("Policy")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _repository;

        public CustomerController(ICustomerRepository repository)
        {
            _repository = repository;
        }


        // GET api/manage/customer/all
        [HttpGet("all")]
        public ActionResult<MultivalueCustomer> GetAllCustomers()
        {

            var response = _repository.GetAllCustomers();
            return Ok(response);

        }

        // POST api/manage/customer/
        [HttpPost]
        public ActionResult<MultivalueCustomer> AddCustomer(Customer customer)
        {
            var response = _repository.AddCustomer(customer);
            return Ok(response);

        }

        // POST api/manage/customer/
        [HttpPatch]
        public ActionResult<MultivalueCustomer> ModifyCustomer(Customer customer)
        {
            var response = _repository.ModifyCustomer(customer);
            return Ok(response);

        }

        // DELETE api/manage/customer/
        [HttpDelete]
        public ActionResult<ActionResponse> DeleteCustomer(CustomerIdRequest deleteId)
        {
            var response = _repository.DeleteCustomer(deleteId);
            return Ok(response);

        }
    }
}
