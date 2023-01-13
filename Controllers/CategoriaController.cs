using IECE_WebApiScanQR.Contexts;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IECE_WebApiScanQR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CategoriaController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Get()
        {
            try
            {
                var categorias = _context.Categoria.ToList();
                return Ok(new
                {
                    error = false,
                    categorias = categorias
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
