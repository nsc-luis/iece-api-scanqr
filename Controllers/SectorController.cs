using IECE_WebApiScanQR.Contexts;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace IECE_WebApiScanQR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SectorController : ControllerBase
    {
        private readonly AppDbContext context;
        public SectorController(AppDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        //[EnableCors("AllowAnyOrigin")]
        public IActionResult Get()
        {
            try
            {
                var sectores = context.Sector.ToList();
                return Ok(new
                {
                    error = false,
                    sectores = sectores
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

        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        [Route("sectoresByDistrito/{idDistrito}")]
        public IActionResult sectoresByDistrito(int idDistrito)
        {
            try
            {
                var sectores = (from s in context.Sector
                                join d in context.Distrito on s.idDistrito equals d.idDistrito
                                where d.idDistrito == idDistrito
                                select s).ToList();
                return Ok(new
                {
                    error = false,
                    sectores = sectores
                });
            }
            catch(Exception e)
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
