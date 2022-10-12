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
    //http://localhost:7163/api/manage/appointment
    [Route("api/manage/appointment")]
    [ApiController]
    [EnableCors("Policy")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository _repository;

        public AppointmentController(IAppointmentRepository repository)
        {
            _repository = repository;
        }

        // POST api/manage/appointment/
        [HttpPost]
        public ActionResult<MultivalueAppointment> GetAllAppointments(NewAppRequest newApp)
        {

            var response = _repository.CreateAppointment(newApp);
            return Ok(response);

        }

        // DELETE api/manage/appointment/
        [HttpDelete]
        public ActionResult<MultivalueAppointment> DeleteAppointment(AppIdRequest deletionId)
        {

            var response = _repository.DeleteAppointment(deletionId);
            return Ok(response);

        }


        // GET api/manage/appointment/all
        [HttpGet("all")]
        public ActionResult<MultivalueAppointment> GetAllAppointments()
        {

            var response = _repository.GetAllAppointments();
            return Ok(response);

        }
    }
}
