using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using DetailTECService.Data;
using DetailTECService.Models;

namespace DetailTECService.Controllers
{
    //LoginController hereda la clase ControllerBase, utilizada para el manejo
    //del endpoints.
    //ApiController identifica a la clase como un controlador en el framework.
    //LoginController se encarga de manejar el endpoint que permite a los usuarios hacer login.
    //Route especifica la ruta para este controlador. En este caso local:
    //http://localhost:5075/api/login
    
    [Route("api/login")]
    [ApiController]
    [EnableCors("Policy")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginRepository _repository;

        public LoginController(ILoginRepository repository)
        {
            _repository = repository;
        }
        
        
        // POST api/login
        [HttpPost]
        public ActionResult<AuthResponse> Authenticate(LoginData loginData)
        {

            var response = _repository.AuthCheck(loginData);
            if(response.logged){
                return Ok(response);
            }
            else
            {
                return Unauthorized(response);
            }
            
            

        }

    }
}