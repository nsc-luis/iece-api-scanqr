using IECE_WebApiScanQR.Contexts;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IECE_WebApiScanQR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DistritoController : ControllerBase
    {
        private readonly AppDbContext context;
        public DistritoController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Get()
        {
            try
            {
                var distritos = context.Distrito.ToList();
                return Ok(new
                {
                    error = false,
                    distritos = distritos
                });
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    error = true,
                    mensaje = e.Message
                });
            }
        }
    }
}
