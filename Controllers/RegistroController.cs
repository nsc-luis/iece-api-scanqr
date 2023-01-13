using IECE_WebApiScanQR.Contexts;
using IECE_WebApiScanQR.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.RegularExpressions;

namespace IECE_WebApiScanQR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RegistroController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Get()
        {
            try
            {
                // CONSULTA QUE INCLUYE DATOS DEL DISTRITO, SECTOR Y CATEGORIA
                var registros = (from r in _context.Registro
                                join d in _context.Distrito on r.idDistrito equals d.idDistrito
                                join c in _context.Categoria on r.idCategoria equals c.idCategoria
                                select new
                                {
                                    d.nombreDistrito,
                                    d.tipoDistrito,
                                    d.numeroDistrito,
                                    r.nombre,
                                    r.apellidoPaterno,
                                    r.apellidoMaterno,
                                    r.bautizado,
                                    r.codigo,
                                    r.verificador,
                                    c.nombreCategoria
                                }).ToList();
                return Ok(new
                {
                    error = false,
                    registro = registros
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

        // OBTIENE DATOS DEL REGISTRO POR SI ID
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        [Route("registroById/{idRegistro}")]
        public IActionResult registroById(int idRegistro)
        {
            try
            {
                // CONSULTA QUE INCLUYE DATOS DEL DISTRITO, SECTOR Y CATEGORIA
                var registro = (from r in _context.Registro
                                join d in _context.Distrito on r.idDistrito equals d.idDistrito
                                join s in _context.Sector on r.idSector equals s.idSector
                                join c in _context.Categoria on r.idCategoria equals c.idCategoria
                                where r.idRegistro == idRegistro
                                select new
                                {
                                    d.nombreDistrito,
                                    d.tipoDistrito,
                                    d.numeroDistrito,
                                    s.numeroSector,
                                    s.tipoSector,
                                    s.nombreSector,
                                    r.nombre,
                                    r.apellidoPaterno,
                                    r.apellidoMaterno,
                                    r.bautizado,
                                    r.codigo,
                                    r.verificador,
                                    c.nombreCategoria
                                }).ToList();
                return Ok(new
                {
                    error = false,
                    registro = registro[0]
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

        // CONSULTA TODOS LOS REGISTROS DE UN DISTRITO
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        [Route("registrosByDistrito/{idDistrito}")]
        public IActionResult registrosByDistrito(int idDistrito)
        {
            try
            {
                // CONSULTA QUE INCLUYE DATOS DEL DISTRITO, SECTOR Y CATEGORIA
                var registros = (from r in _context.Registro
                                 join d in _context.Distrito on r.idDistrito equals d.idDistrito
                                 join s in _context.Sector on r.idSector equals s.idSector
                                 join c in _context.Categoria on r.idCategoria equals c.idCategoria
                                 where r.idDistrito == idDistrito
                                 select new
                                 {
                                     d.nombreDistrito,
                                     d.tipoDistrito,
                                     d.numeroDistrito,
                                     s.numeroSector,
                                     s.tipoSector,
                                     s.nombreSector,
                                     r.nombre,
                                     r.apellidoPaterno,
                                     r.bautizado,
                                     r.codigo,
                                     c.nombreCategoria
                                 }).ToList();
                return Ok(new
                {
                    error = false,
                    registros = registros
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

        // AGREGA UN NUEVO REGISTRO PARA GENERAR QR
        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Post([FromBody] Registro info)
        {
            try
            {
                // RENERA CADENA PARA VERIFICACION DE DUPLICADOS
                var pattern = @"[^aeiou|AEIOU]";
                Regex rg = new Regex(pattern, RegexOptions.IgnoreCase);
                var vap = Regex.Replace(info.apellidoPaterno.ToUpper(), pattern, "");
                char[] ap = info.apellidoPaterno.ToUpper().ToArray();
                info.apellidoMaterno = info.apellidoMaterno == "" ? "N" : info.apellidoMaterno.ToUpper();
                info.codigo = info.codigo == "" || info.codigo == null ? "NoGenerado" : info.codigo;
                char[] am = info.apellidoMaterno.ToUpper().ToArray();
                char[] n = info.nombre.ToUpper().ToArray();
                var pvap = ap[0] == vap[0] ? vap[1] : vap[0];
                int bautizado = info.bautizado ? 1 : 0;
                var verificador = $"{ap[0].ToString()}{pvap.ToString()}{am[0].ToString()}{n[0].ToString()}{info.idDistrito}{info.idSector}{info.idCategoria}{bautizado}";

                // COMPRUEBA SI EXISTE O NO ALGUNA PERSONA DUPLICADA EN EL REGISTRO CON LA CADENA DE VERIFICACION
                var query = _context.Registro.FirstOrDefault(r => r.verificador == verificador);
                if (query != null && query.verificador == verificador)
                {
                    return Ok(new
                    {
                        error = true,
                        mensaje = "SE A ENCONTRADO UN REGISTRO DUPLICADO, FAVOR DE VOLVER A INTENTAR."
                    });
                }
                else
                {
                    // ALTA DE REGISTRO
                    var registro = new Registro
                    {
                        idSector = info.idSector,
                        idDistrito = info.idDistrito,
                        nombre = info.nombre.ToUpper(),
                        apellidoPaterno = info.apellidoPaterno.ToUpper(),
                        apellidoMaterno = info.apellidoMaterno.ToUpper(),
                        idCategoria = info.idCategoria,
                        bautizado = info.bautizado,
                        codigo = "No asignado",
                        verificador = verificador
                    };
                    _context.Registro.Add(registro);
                    _context.SaveChanges();

                    // ACTUALIZACION DEL REGISTRO CON SU NUEVO CODIGO PARA QR
                    var b = registro.bautizado ? 1 : 0;
                    registro.codigo = $"{registro.verificador}|{registro.idRegistro}|{b}|{registro.idCategoria}|{registro.idSector}|{registro.idDistrito}";
                    _context.Registro.Update(registro);
                    _context.SaveChanges();

                    var qNvoRegistro = (from r in _context.Registro
                                    join d in _context.Distrito on r.idDistrito equals d.idDistrito
                                    join s in _context.Sector on r.idSector equals s.idSector
                                    join c in _context.Categoria on r.idCategoria equals c.idCategoria
                                    where r.idRegistro == registro.idRegistro
                                    select new
                                    {
                                        d.nombreDistrito,
                                        d.tipoDistrito,
                                        d.numeroDistrito,
                                        s.numeroSector,
                                        s.tipoSector,
                                        s.nombreSector,
                                        r.nombre,
                                        r.apellidoPaterno,
                                        r.apellidoMaterno,
                                        r.bautizado,
                                        r.codigo,
                                        r.verificador,
                                        c.nombreCategoria
                                    }).ToList();

                    return Ok(new
                    {
                        error = false,
                        registro = qNvoRegistro[0]
                    });
                }
            }
            catch(Exception e)
            {
                return Ok(new
                {
                    error = true,
                    mensaje = e
                });
            }
        }
    }
}
