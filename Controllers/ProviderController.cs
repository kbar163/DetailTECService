using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using DetailTECService.Models;
using DetailTECService.Data;

namespace TallerTECService.Controllers
{

    //ProviderController hereda la clase ControllerBase, utilizada para el manejo
    //del endpoints.
    //ApiController identifica a la clase como un controlador en el framework.
    //ProviderController Se encarga de manejar operaciones CRUD para las sucursales registrados.
    //Route especifica la ruta para este controlador. En este caso local:
    //http://localhost:7163/api/manage/provider
    [Route("api/manage/provider")]
    [ApiController]
    [EnableCors("Policy")]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderRepository _repository;

        public ProviderController(IProviderRepository repository)
        {
            _repository = repository;
        }

        // GET api/manage/provider/all
        [HttpGet("all")]
        public ActionResult<MultivalueProvider> GetAllProviders()
        {

            var response = _repository.GetAllProviders();
            return Ok(response);

        }

        // POST api/manage/provider
        [HttpPost]
        public ActionResult<ActionResponse> AddProvider(Provider newProvider)
        {

            var response = _repository.AddProvider(newProvider);
            return Ok(response);


        }

        // PATCH api/manage/provider
        [HttpPatch]
        public ActionResult<ActionResponse> ModifyOffice(Provider newProvider)
        {

            var response = _repository.ModifyProvider(newProvider);
            return Ok(response); 

        }

        // DELETE api/manage/provider
        [HttpDelete]
        public ActionResult<ActionResponse> DeleteProvider(ProviderIdRequest deletionId)
        {

            var response = _repository.DeleteProvider(deletionId);
            return Ok(response);

        }


    }
}