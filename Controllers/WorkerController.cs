using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using DetailTECService.Models;
using DetailTECService.Data;

namespace DetailTECService.Controllers
{

    //WorkerController hereda la clase ControllerBase, utilizada para el manejo
    //del endpoints.
    //ApiController identifica a la clase como un controlador en el framework.
    //WorkerController Se encarga de manejar operaciones CRUD para los trabajadores registrados.
    //Route especifica la ruta para este controlador. En este caso local:
    //http://localhost:7163/api/manage/worker
    [Route("api/manage/worker")]
    [ApiController]
    [EnableCors("Policy")]
    public class WorkerController : ControllerBase
    {
        private readonly IWorkerRepository _repository;

        public WorkerController(IWorkerRepository repository)
        {
            _repository = repository;
        }

        // POST api/manage/worker
        [HttpPost]
        public ActionResult<ActionResponse> AddWorker(Worker newWorker)
        {

            var response = _repository.AddWorker(newWorker);
            return Ok(response);
             

        }

        // GET api/manage/worker/all
        [HttpGet("all")]
        public ActionResult<MultivalueWorker> GetAllWorkers()
        {

            var response = _repository.GetAllWorkers();
            return Ok(response);

        }

        // PATCH api/manage/worker
        [HttpPatch]
        public ActionResult<ActionResponse> ModifyWorker(Worker newWorker)
        {

            var response = _repository.ModifyWorker(newWorker);
            return Ok(response); 

        }

        // DELETE api/manage/worker
        [HttpDelete]
        public ActionResult<ActionResponse> DeleteWorker(WorkerIdRequest deletionId)
        {
            
            var response = _repository.DeleteWorker(deletionId);
            return Ok(response);

        }
        
        // GET api/manage/worker/roles
        [HttpGet("roles")]
        public ActionResult<MultivalueRole> GetRoles()
        {

            var response = _repository.GetRoles();
            return Ok(response);

        }

        // GET api/manage/worker/payment/types
        [HttpGet("payment/types")]
        public ActionResult<MultivalueRole> GetPaymentTypes()
        {

            var response = _repository.GetPaymentTypes();
            return Ok(response);

        }

        

    }
}
