using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using DetailTECService.Models;
using DetailTECService.Data;

namespace DetailTECService.Controllers
{

    //OfficeController hereda la clase ControllerBase, utilizada para el manejo
    //del endpoints.
    //ApiController identifica a la clase como un controlador en el framework.
    //OfficeController Se encarga de manejar operaciones CRUD para las sucursales registrados.
    //Route especifica la ruta para este controlador. En este caso local:
    //http://localhost:7163/api/manage/office
    [Route("api/manage/office")]
    [ApiController]
    [EnableCors("Policy")]
    public class OfficeController : ControllerBase
    {
        private readonly IOfficeRepository _repository;

        public OfficeController(IOfficeRepository repository)
        {
            _repository = repository;
        }

        // GET api/manage/office/all
        [HttpGet("all")]
        public ActionResult<MultivalueOffice> GetAllOffices()
        {

            var response = _repository.GetAllOffices();
            return Ok(response);

        }

        // POST api/manage/office
        [HttpPost]
        public ActionResult<ActionResponse> AddOffice(Office newOffice)
        {

            var response = _repository.AddOffice(newOffice);
            return Ok(response);
             

        }

        // PATCH api/manage/office
        [HttpPatch]
        public ActionResult<ActionResponse> ModifyOffice(Office newOffice)
        {

            var response = _repository.ModifyOffice(newOffice);
            return Ok(response); 

        }

        // DELETE api/manage/office
        [HttpDelete]
        public ActionResult<ActionResponse> DeleteOffice(OfficeIdRequest deletionId)
        {
            
            var response = _repository.DeleteOffice(deletionId);
            return Ok(response);

        }

    }
}
