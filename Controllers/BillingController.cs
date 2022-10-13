using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using DetailTECService.Models;
using DetailTECService.Data;

namespace DetailTECService.Controllers
{

    //AppointmentController hereda la clase ControllerBase, utilizada para el manejo
    //del endpoints.
    //ApiController identifica a la clase como un controlador en el framework.
    //AppointmentController Se encarga de manejar operaciones CRUD para la gestion de citas.
    //Route especifica la ruta para este controlador. En este caso local:
    //http://localhost:7163/api/manage/billing
    [Route("api/manage/billing")]
    [ApiController]
    [EnableCors("Policy")]
    public class BillingController : ControllerBase
    {
        private readonly IBillingRepository _repository;

        public BillingController(IBillingRepository repository)
        {
            _repository = repository;
        }

        // POST api/manage/appointment/
        [HttpPost]
        public ActionResult<ActionResponse> CreateBill(BillRequest newBill)
        {

            var response = _repository.CreateBill(newBill);
            return Ok(response);

        }
    }
}