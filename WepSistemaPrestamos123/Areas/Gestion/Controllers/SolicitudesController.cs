using Comun.DTO.Solicitud;
using Datos.Contratos.Solicitud;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace WepPrestamos.Areas.Gestion.Controllers
{
    [Area("Gestion")]
    //[Authorize]
    public class SolicitudesController : Controller
    {
        private readonly IRegistroSolicitud _solicitud;
        private readonly IBLConsultar_Detalle_Master _dominio;
        private readonly ILugaresGeograficos _lugares;


        public SolicitudesController(IRegistroSolicitud solicitud, IBLConsultar_Detalle_Master dominio, ILugaresGeograficos lugares)
        {
            _solicitud = solicitud;
            _dominio = dominio;
            _lugares = lugares;

        }
        public async Task<IActionResult> Index()
        {
            var _departamentos = await _lugares.ObtenerAsync<Parametros_Consulta_Lugares_Geograficos_DTO, List<Respuesta_Consulta_Lugares_Geograficos_DTO>>(new Parametros_Consulta_Lugares_Geograficos_DTO { TIPO_LUGAR = "DE" });
            Parametros_Consulta_Solicitudes_X_Estado_Dto parPendiente = new Parametros_Consulta_Solicitudes_X_Estado_Dto();
            parPendiente.ID_ESTADO = 12; //Registrada

            Parametros_Consulta_Solicitudes_X_Estado_Dto parRevi = new Parametros_Consulta_Solicitudes_X_Estado_Dto();
            parRevi.ID_ESTADO = 13; //En Revision

            var listSolregis = await _solicitud.ObtenerAsync<Parametros_Consulta_Solicitudes_X_Estado_Dto,List<Respuesta_Consulta_Solicitudes_Prestamo_Dto>> (parPendiente);
            var listSolRevis = await _solicitud.ObtenerAsync<Parametros_Consulta_Solicitudes_X_Estado_Dto, List<Respuesta_Consulta_Solicitudes_Prestamo_Dto>>(parRevi);
            var listFull = await _solicitud.ObtenerAsync<List<Respuesta_Consulta_Solicitudes_Prestamo_Dto>>();

            ViewBag.listSolregis = listSolregis.Respuesta;
            ViewBag.listSolRevis = listSolRevis.Respuesta;
            ViewBag.listSolFull = listFull.Respuesta;
            ViewBag.ListaDepartamentos = new SelectList(_departamentos.Respuesta, "CodigoDane", "Descripcion");

            return View();
        }

        public async Task<IActionResult> Validar(Int64 id, Int64 estadoId)
        {
            try
            {
                
                if (id != null & id > 0 & estadoId != 0)
                {
                   
                    var _listaTipoDoc = await _dominio.ListaDetalle(1);
                    var _departamentos = await _lugares.ObtenerAsync<Parametros_Consulta_Lugares_Geograficos_DTO, List<Respuesta_Consulta_Lugares_Geograficos_DTO>>(new Parametros_Consulta_Lugares_Geograficos_DTO { TIPO_LUGAR = "DE" });
                    

                    ViewBag.listTipoDocumentos = new SelectList(_listaTipoDoc, "Id", "Nombre");
                    ViewBag.ListaDepartamentos = new SelectList(_departamentos.Respuesta, "CodigoDane", "Descripcion");
                   

                    var solDto = await _solicitud.Obtener_X_Id_Async<Int64, Respuesta_Consulta_Solicitudes_Prestamo_Dto>(id);

                    long _idDaneDepto = solDto.Respuesta.DepartamentoResidenciaId;
                    long _idDaneMpio = solDto.Respuesta.MunicipioResidenciaId;
                    var _lista_Municipio = await _lugares.ObtenerAsync<Parametros_Consulta_Lugares_Geograficos_DTO, List<Respuesta_Consulta_Lugares_Geograficos_DTO>>(new Parametros_Consulta_Lugares_Geograficos_DTO { TIPO_LUGAR = "MU", ID_DANE_PADRE = _idDaneDepto});
                    var _lista_Barrios = await _lugares.ObtenerBarriosAsync<Parametros_Consulta_Barrios_Dto, List<Respuesta_Consulta_Barrios_Dto>>(new Parametros_Consulta_Barrios_Dto {ID_DANE_MUNICIPIO = _idDaneMpio });


                    ViewBag.ListaMunicipios = new SelectList(_lista_Municipio.Respuesta, "CodigoDane", "Descripcion");
                    ViewBag.ListaBarrios = new SelectList(_lista_Barrios.Respuesta, "Id", "Nombre");
                    
                    //Si el estado de la solicitud es 12 entonces actualizar a 13

                    return View(solDto.Estado? solDto.Respuesta:new Respuesta_Consulta_Solicitudes_Prestamo_Dto());
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return NotFound();
            }            
        }
    }
}
