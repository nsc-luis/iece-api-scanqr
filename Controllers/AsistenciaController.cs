using IECE_WebApiScanQR.Contexts;
using IECE_WebApiScanQR.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace IECE_WebApiScanQR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsistenciaController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly DateTime fechaActual = DateTime.Now;

        public AsistenciaController(AppDbContext context)
        {
            _context = context;
        }

        // AGREGA UN NUEVO REGISTRO DE ASISTENCIA
        [HttpPost]
        [EnableCors("AllowAnyOrigin")]
        public IActionResult Post([FromBody] string cadena)
        {
            try
            {
                // OBTINE FECHA DEL DIA ACTUAL
                var hoy = DateOnly.FromDateTime(DateTime.Now);

                // REVISA SI LA CADENA DEL QR ES VALIDA
                string[] datos = cadena.Split("|");
                if (datos.Length >= 2)
                {
                    var registro = _context.Registro.FirstOrDefault(r => r.idRegistro == int.Parse(datos[1]));
                    if (registro != null && registro.verificador == datos[0])
                    {
                        // VERIFICA SI LA PERSONA YA TIENE ASISTENCIA EL DIA ACTUAL
                        var verificaAsistencia = (from a in _context.Asistencia
                                                  where a.idRegistro == int.Parse(datos[1])
                                                  && a.fecha >= DateTime.Parse($"{hoy.ToString()} 00:00:00")
                                                  && a.fecha <= DateTime.Parse($"{hoy.ToString()} 23:59:59")
                                                  select a).ToList();
                        if (verificaAsistencia.Any())
                        {
                            // OBTIENE LOS DATOS DE LA PERSONA QUE YA REGISTRO ASISTENCIA EL DIA ACTUAL
                            if (registro.idSector == 0)
                            {
                                var data1 = (from r in _context.Registro
                                             join d in _context.Distrito on r.idDistrito equals d.idDistrito
                                             join c in _context.Categoria on r.idCategoria equals c.idCategoria
                                             where r.idRegistro == registro.idRegistro
                                             select new
                                             {
                                                 d.nombreDistrito,
                                                 d.tipoDistrito,
                                                 d.numeroDistrito,
                                                 r.nombre,
                                                 r.apellidoPaterno,
                                                 r.bautizado,
                                                 r.codigo,
                                                 r.verificador,
                                                 c.nombreCategoria
                                             }).ToList();
                                return Ok(new
                                {
                                    error = false,
                                    registro = data1
                                });
                            }
                            else
                            {
                                var data1 = (from r in _context.Registro
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
                                                 r.bautizado,
                                                 r.codigo,
                                                 r.verificador,
                                                 c.nombreCategoria
                                             }).ToList();
                                return Ok(new
                                {
                                    error = false,
                                    registro = data1
                                });
                            }
                        }
                        // REGISTRA ASISTENCIA DE LA PERSONA DEL QR
                        var asist = new Asistencia
                        {
                            idDistrito = registro.idDistrito,
                            idSector = registro.idSector,
                            idRegistro = registro.idRegistro,
                            idCategoria = registro.idCategoria,
                            bautizado = registro.bautizado,
                            fecha = fechaActual
                        };
                        _context.Asistencia.Add(asist);
                        _context.SaveChanges();

                        // OBTIENE LOS DATOS DE LA PERSONA QUE REGISTRO ASISTENCIA
                        var data = (from r in _context.Registro
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
                                        r.bautizado,
                                        r.codigo,
                                        r.verificador,
                                        c.nombreCategoria
                                    }).ToList();
                        return Ok(new
                        {
                            error = false,
                            registro = data
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            error = true,
                            mensaje = "NO SE ENCONTRO REGISTRO. FAVOR DE PASAR AL MODULO DE REGISTROS."
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        error = true,
                        mensaje = "NO SE ENCONTRO REGISTRO. FAVOR DE PASAR AL MODULO DE REGISTROS."
                    });
                }
            }
            catch (Exception e)
            {
                return Ok(new
                {
                    error = true,
                    mensaje = e
                });
            }
        }

        // OBTIENE TODA LA ASISTENCIA DE UN DÍA EN ESPECIFICO
        [HttpGet]
        [EnableCors("AllowAnyOrigin")]
        [Route("asistenciaPorFecha/{fecha}")]
        public IActionResult asistenciaPorFecha(string fecha)
        {
            try
            {
                var asistencia = (from a in _context.Asistencia
                                  where a.fecha >= DateTime.Parse($"{fecha} 00:00:00")
                                  && a.fecha <= DateTime.Parse($"{fecha} 23:59:59")
                                  select a).ToList();
                return Ok(new
                {
                    error = false,
                    asistencia = asistencia
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
