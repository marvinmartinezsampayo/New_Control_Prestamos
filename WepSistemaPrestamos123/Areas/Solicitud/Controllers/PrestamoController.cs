using Comun.DTO.Solicitud;
using Comun.Enumeracion;
using Comun.Generales;
using Datos.Contratos.Solicitud;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json;

namespace WepPrestamos.Areas.Solicitud.Controllers
{
    [Area("Solicitud")]
    //[Authorize]
    public class PrestamoController : Controller
    {
        private readonly ILogger<PrestamoController> _logger;
        private readonly ILugaresGeograficos _lugares;
        private readonly IConfiguration _configuration;
        private readonly IGenerar_Codigo _codigo;
        private string original;
        private string key = "P7JWwviSTY%YTnNK%cCQJYFu3eu#6LrU$9%PysY4No8TCuTR6X";
        public PrestamoController
            (
            IConfiguration configuration,
            ILogger<PrestamoController> logger,
            ILugaresGeograficos lugares,
            IGenerar_Codigo codigo
            )
        {
            _configuration = configuration;
            _logger = logger;
            _lugares = lugares;
            _codigo = codigo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Solicitar()
        {
            Parametros_Consulta_Lugares_Geograficos_DTO p_lugares = new Parametros_Consulta_Lugares_Geograficos_DTO();

            p_lugares.TIPO_LUGAR = "DE";
            var listDepto = await _lugares.ObtenerAsync<Parametros_Consulta_Lugares_Geograficos_DTO,List<Respuesta_Consulta_Lugares_Geograficos_DTO>>(p_lugares);

            ViewBag.listaDepto = new SelectList(listDepto.Respuesta, "Id", "Descripcion");

            return View();
        }

        public async Task<IActionResult> Registro(string json)
        {
            try
            {
                string decript = Encryption.DecryptString(json, key);
                RespuestaDto<Respuesta_Consulta_Codigo_Acceso_DTO> respCodigo = JsonConvert.DeserializeObject<RespuestaDto<Respuesta_Consulta_Codigo_Acceso_DTO>>(decript);

                if (respCodigo.Codigo == EstadoOperacion.Bueno && respCodigo.Respuesta.Codigo != null && respCodigo.Respuesta.Codigo != "")
                {
                    var cod = await _codigo.ObtenerAsync<string, Respuesta_Consulta_Codigo_Acceso_DTO>(respCodigo.Respuesta.Codigo);

                    if (cod.Respuesta != null && cod.Respuesta.FechaFin < DateTime.Now && cod.Respuesta.Habilitado) 
                    {
                        long CantRegistros = cod.Respuesta.CantidadRegistros;


                        //Aca contamos la cantidad de solicitudes registradas con ese codigo

                        //



                        ViewBag.ObjCodigo = cod.Respuesta;                        
                    }
                    else
                    {
                        //Redireccionamos a la vista Solicitar, indicando que el codigo no es valido
                    }

                    




                   
                }
                else
                {

                }
            }
            catch (Exception)
            {

                throw;
            }



            return View();
        }

        //public IActionResult Registro()
        //{
        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> ValidarCodigo(string _codigoUser)
        {
            string encrypted = "error";
            RespuestaDto<string> respuestaDto = new RespuestaDto<string>();
            try
            {

                var cod = await _codigo.ObtenerAsync<string, Respuesta_Consulta_Codigo_Acceso_DTO>(_codigoUser);

                if(cod.Respuesta.Codigo == null)
                {
                    respuestaDto = new RespuestaDto<string>() {Codigo= EstadoOperacion.Malo, Mensaje = "Codigo no encontrado" };
                    return RetornoRespuesta<RespuestaDto<string>>(respuestaDto, EstadoOperacion.Malo);
                }

                encrypted = Encryption.EncryptString(JsonConvert.SerializeObject(cod), key);
                respuestaDto = new RespuestaDto<string>()
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje= "Codigo encontrado",
                    Respuesta = encrypted
                };


                return RetornoRespuesta<RespuestaDto<string>>(respuestaDto, EstadoOperacion.Bueno);
               
            }
            catch (Exception ex)
            {
                RespuestaDto<Respuesta_Consulta_Codigo_Acceso_DTO> respDto = new RespuestaDto<Respuesta_Consulta_Codigo_Acceso_DTO>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = $"Ocurrio exepción {ex.Message} - {ex.InnerException}"
                };
               
                return RetornoRespuesta<RespuestaDto<Respuesta_Consulta_Codigo_Acceso_DTO>>(respDto, EstadoOperacion.Bueno);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Generar_Codigo_Acceso()
        {
            try
            {
                var cod = await _codigo.ObtenerAsync<string>();
                return RetornoRespuesta<RespuestaDto<string>>(cod, EstadoOperacion.Bueno);
            }
            catch (Exception ex)
            {
                RespuestaDto<string> respDto = new RespuestaDto<string>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = $"Ocurrio exepción {ex.Message} - {ex.InnerException}"
                };
                return RetornoRespuesta<RespuestaDto<string>>(respDto, EstadoOperacion.Bueno);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DesencriptarCodigo(string _codigoEncript)
        {
            try
            {
                RespuestaDto<string> resp = new RespuestaDto<string>();
                string dec = Encryption.DecryptString(_codigoEncript, key);

                return Ok(dec);
            }
            catch (Exception ex)
            {
                return NotFound("Error");
            }
        }


        public IActionResult RetornoRespuesta<TParam>(TParam _parametro, EstadoOperacion _codigoRespuesta)
        {
            switch (_codigoRespuesta)
            {
                case EstadoOperacion.Bueno:
                    return Ok(_parametro);

                case EstadoOperacion.Malo:
                    return Ok(_parametro);

                case EstadoOperacion.Excepcion:
                    return StatusCode(StatusCodes.Status500InternalServerError, _parametro);

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, _parametro);
            }
        }

    }
}
