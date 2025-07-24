using Comun.DTO.Prestamo;
using Comun.DTO.Solicitud;
using Comun.Enumeracion;
using Comun.Generales;
using Datos.Contexto;
using Datos.Contratos.Login;
using Datos.Contratos.Prestamo;
using Datos.Contratos.Solicitud;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Gestion
{
    public class GestionPrestamo : IGestionPrestamo
    {
        private readonly IConfiguration _configuration;       
        private readonly ContextoGeneral _context;

        public GestionPrestamo(IConfiguration configuration, ContextoGeneral context)
        {   
            _configuration = configuration;
            _context= context;
        }

        public async Task<RespuestaDto<TReturn>> Obtener_X_Identificacion_Async<TParam, TReturn>(TParam _modelo)
        {
            try
            {
                List<Prestamo_Dto> resp = new List<Prestamo_Dto>();

                if (_modelo is Int64 IdSolicitud)
                {
                    var estadosExcluidos = new List<long> { 52, 54 };
                    var consulta = await _context.PRESTAMOS
                            .Include(p => p.FK_ID_PERIODICIDAD)  // Relación de navegación a periodicidad
                            .Include(p => p.FK_ID_ESTADO)        // Relación de navegación a estado
                            .Include(p => p.FK_ID_SOLICITUD)        // Relación con la solicitud
                            .Where(p => !estadosExcluidos.Contains(p.ID_ESTADO) && p.FK_ID_SOLICITUD.NumeroIdentificacion == IdSolicitud)
                            .Select(p => new Prestamo_Dto
                            {
                                ID = p.ID,
                                ID_SOLICITUD = p.ID_SOLICITUD,
                                SOLICITANTE = (p.FK_ID_SOLICITUD.PrimerNombreSolicitante ?? "") + " " +  (p.FK_ID_SOLICITUD.SegundoNombreSolicitante ?? "") + " " + (p.FK_ID_SOLICITUD.PrimerApellidoSolicitante ?? "") + " " + (p.FK_ID_SOLICITUD.SegundoApellidoSolicitante ?? ""),
                                MONTO = p.MONTO,
                                NUMERO_CUOTAS = p.NUMERO_CUOTAS,
                                ID_PERIODICIDAD = p.ID_PERIODICIDAD,
                                PERIODICIDAD = p.FK_ID_PERIODICIDAD.Nombre,
                                INTERES = p.INTERES,
                                FECHA_INICIO = p.FECHA_INICIO,
                                FECHA_FIN = p.FECHA_FIN,
                                SALDO_MONTO = p.SALDO_MONTO,
                                ID_ESTADO = p.ID_ESTADO,
                                ESTADO = p.FK_ID_ESTADO.Nombre
                            })
                            .ToListAsync();

                    return new RespuestaDto<TReturn>
                    {
                        Codigo = EstadoOperacion.Bueno,
                        Mensaje = "OK",
                        Respuesta = (TReturn)Convert.ChangeType(consulta, typeof(TReturn))
                    };
                }
                else
                {
                    return new RespuestaDto<TReturn>
                    {
                        Codigo = EstadoOperacion.Malo,
                        Mensaje = "ERROR",
                        Respuesta = (TReturn)Convert.ChangeType(resp, typeof(TReturn))
                    };
                }
            }
            catch (Exception)
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "Excepcion"                   
                };
            }
        }
    }
}
