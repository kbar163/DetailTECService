using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using DetailTECService.Models;
using DetailTECService.Data;

namespace DetailTECService.Controllers
{

    //WorkerController hereda la clase ControllerBase, utilizada para el manejo
    //de endpoints.
    //ApiController identifica a la clase como un controlador en el framework.
    //WashController Se encarga de manejar operaciones CRUD para los tipos de lavados registrados.
    //Route especifica la ruta para este controlador. En este caso local:
    //http://localhost:7163/api/manage/wash
    [Route("api/manage/wash")]
    [ApiController]
    [EnableCors("Policy")]
    public class WashController : ControllerBase
    {
        private readonly IWashRepository _repository;

        public WashController(IWashRepository repository)
        {
            _repository = repository;
        }


        // GET api/manage/wash/all
        [HttpGet("all")]
        public ActionResult<MultivalueWash> GetAllWashTypes()
        {

            var response = _repository.GetAllWashTypes();
            return Ok(response);

        }
    }
}