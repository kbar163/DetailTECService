using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using DetailTECService.Models;
using DetailTECService.Data;

namespace DetailTECService.Controllers
{

    //BillingController hereda la clase ControllerBase, utilizada para el manejo
    //del endpoints.
    //ApiController identifica a la clase como un controlador en el framework.
    //BillingController Se encarga de manejar operaciones CRUD para la gestion de facturacion.
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

        // POST api/manage/billing/
        [HttpPost]
        public ActionResult CreateBill(BillRequest newBill)
        {
            //El codigo comentado sirve para enviar el archivo como respuesta del request
            //_repository.CreateBill(newBill)
            // var path = @$"Data/File Generation/Generated/Bills/factura-{newBill.id_cita}.pdf";
            // var memory = new MemoryStream();
            // using (var stream = new FileStream(path,FileMode.Open))
            // {
            //     stream.CopyTo(memory);
            // }
            // memory.Position = 0;
            // return File(memory,"application/pdf",Path.GetFileName(path));

            var response = _repository.CreateBill(newBill);
            return Ok(response);
        }
    }
}