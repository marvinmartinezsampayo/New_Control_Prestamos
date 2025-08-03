using Comun.DTO.Prestamo;
using Comun.DTO.Solicitud;
using Comun.Enumeracion;
using Comun.Generales;
using Datos.Contexto;
using Datos.Contratos.Login;
using Datos.Contratos.Prestamo;
using Datos.Contratos.Solicitud;
using Datos.Modelos;
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
            _context = context;
        }

        public async Task<RespuestaDto<TReturn>> Insertar_Pago_Async<TParam, TReturn>(TParam _modelo)
        {
            try
            {
                if (_modelo is RegistrarActualizarPagoDto pago)
                {
                    var nuevoPago = new PAGOS
                    {
                        ID_PRESTAMO = pago.ID_PRESTAMO,
                        FECHA_PAGO = pago.FECHA_PAGO,
                        MONTO = pago.MONTO,
                        ID_TIPO_PAGO = pago.ID_TIPO_PAGO
                    };

                    await _context.PAGOS.AddAsync(nuevoPago);                    
                    await  _context.SaveChangesAsync();

                    return new RespuestaDto<TReturn>
                    {
                        Codigo = EstadoOperacion.Bueno,
                        Mensaje = "OK",
                        Respuesta = (TReturn)Convert.ChangeType(true, typeof(TReturn))
                    };
                }
                else
                {
                    return new RespuestaDto<TReturn>
                    {
                        Codigo = EstadoOperacion.Malo,
                        Mensaje = "ERROR",
                        Respuesta = (TReturn)Convert.ChangeType(_modelo, typeof(TReturn))
                    };
                }

            }
            catch (Exception ex)
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "Excepcion"
                };
            }
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
                        .Include(p => p.FK_ID_SOLICITUD)     // Relación con la solicitud
                        .Where(p => !estadosExcluidos.Contains(p.ID_ESTADO) && p.FK_ID_SOLICITUD.NumeroIdentificacion == IdSolicitud)
                        .Select(p => new Prestamo_Dto
                        {
                            ID = p.ID,
                            ID_SOLICITUD = p.ID_SOLICITUD,
                            SOLICITANTE = (p.FK_ID_SOLICITUD.PrimerNombreSolicitante ?? "") + " " +
                                         (p.FK_ID_SOLICITUD.SegundoNombreSolicitante ?? "") + " " +
                                         (p.FK_ID_SOLICITUD.PrimerApellidoSolicitante ?? "") + " " +
                                         (p.FK_ID_SOLICITUD.SegundoApellidoSolicitante ?? ""),
                            MONTO = p.MONTO,
                            NUMERO_CUOTAS = p.NUMERO_CUOTAS,
                            ID_PERIODICIDAD = p.ID_PERIODICIDAD,
                            PERIODICIDAD = p.FK_ID_PERIODICIDAD.Nombre,
                            INTERES = p.INTERES,
                            FECHA_INICIO = p.FECHA_INICIO,
                            FECHA_FIN = p.FECHA_FIN,
                            SALDO_MONTO = p.SALDO_MONTO,
                            ID_ESTADO = p.ID_ESTADO,
                            ESTADO = p.FK_ID_ESTADO.Nombre,

                            // Conteo de pagos realizados para este préstamo
                            CANTIDAD_PAGOS = _context.PAGOS.Count(pg => pg.ID_PRESTAMO == p.ID),

                            // Monto total pagado (opcional)
                            MONTO_TOTAL_PAGADO = _context.PAGOS
                                .Where(pg => pg.ID_PRESTAMO == p.ID)
                                .Sum(pg => (long?)pg.MONTO) ?? 0,

                            // Número de la última cuota pagada (opcional)
                            ULTIMA_CUOTA_PAGADA = _context.PAGOS
                                .Where(pg => pg.ID_PRESTAMO == p.ID)
                                .Max(pg => (int?)pg.NUMERO_CUOTA) ?? 0,

                            // Fecha del último pago (opcional)
                            FECHA_ULTIMO_PAGO = _context.PAGOS
                                .Where(pg => pg.ID_PRESTAMO == p.ID)
                                .Max(pg => (DateTime?)pg.FECHA_PAGO),

                            // Saldo pendiente calculado (opcional)
                            SALDO_PENDIENTE = p.MONTO - (_context.PAGOS
                                .Where(pg => pg.ID_PRESTAMO == p.ID)
                                .Sum(pg => (long?)pg.MONTO) ?? 0)
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

        public async Task<RespuestaDto<TReturn>> Obtener_X_ID_Async<TParam, TReturn>(TParam _modelo)
        {
            try
            {
                List<Prestamo_Dto> resp = new List<Prestamo_Dto>();

                if (_modelo is Int64 IdPrestamo)
                {

                    var prestamo = await _context.PRESTAMOS
                                .Include(p => p.FK_ID_PERIODICIDAD)
                                .Include(p => p.FK_ID_ESTADO)
                                .Include(p => p.FK_ID_SOLICITUD)
                                .Where(p => p.ID == IdPrestamo)
                                .Select(p => new Prestamo_Dto
                                {
                                    ID = p.ID,
                                    ID_SOLICITUD = p.ID_SOLICITUD,
                                    SOLICITANTE = (p.FK_ID_SOLICITUD.PrimerNombreSolicitante ?? "") + " " +
                                         (p.FK_ID_SOLICITUD.SegundoNombreSolicitante ?? "") + " " +
                                         (p.FK_ID_SOLICITUD.PrimerApellidoSolicitante ?? "") + " " +
                                         (p.FK_ID_SOLICITUD.SegundoApellidoSolicitante ?? ""),
                                    MONTO = p.MONTO,
                                    NUMERO_CUOTAS = p.NUMERO_CUOTAS,
                                    ID_PERIODICIDAD = p.ID_PERIODICIDAD,
                                    PERIODICIDAD = p.FK_ID_PERIODICIDAD.Nombre,
                                    INTERES = p.INTERES,
                                    FECHA_INICIO = p.FECHA_INICIO,
                                    FECHA_FIN = p.FECHA_FIN,
                                    SALDO_MONTO = p.SALDO_MONTO,
                                    ID_ESTADO = p.ID_ESTADO,
                                    ESTADO = p.FK_ID_ESTADO.Nombre,
                                    // Conteo de pagos realizados para este préstamo
                                    CANTIDAD_PAGOS = _context.PAGOS.Count(pg => pg.ID_PRESTAMO == p.ID),

                                    // Monto total pagado (opcional)
                                    MONTO_TOTAL_PAGADO = _context.PAGOS
                                    .Where(pg => pg.ID_PRESTAMO == p.ID)
                                    .Sum(pg => (long?)pg.MONTO) ?? 0,

                                    // Número de la última cuota pagada (opcional)
                                    ULTIMA_CUOTA_PAGADA = _context.PAGOS
                                    .Where(pg => pg.ID_PRESTAMO == p.ID)
                                    .Max(pg => (int?)pg.NUMERO_CUOTA) ?? 0,

                                    // Fecha del último pago (opcional)
                                    FECHA_ULTIMO_PAGO = _context.PAGOS
                                    .Where(pg => pg.ID_PRESTAMO == p.ID)
                                    .Max(pg => (DateTime?)pg.FECHA_PAGO),

                                    // Saldo pendiente calculado (opcional)
                                    SALDO_PENDIENTE = p.MONTO - (_context.PAGOS
                                    .Where(pg => pg.ID_PRESTAMO == p.ID)
                                    .Sum(pg => (long?)pg.MONTO) ?? 0)
                                })
                                .FirstOrDefaultAsync();

                    if (prestamo == null)
                    {
                        return new RespuestaDto<TReturn>
                        {
                            Codigo = EstadoOperacion.Malo,
                            Mensaje = $"No se encontró un préstamo con el ID: {IdPrestamo}",
                            Respuesta = (TReturn)Convert.ChangeType(resp, typeof(TReturn))
                        };
                    }

                    return new RespuestaDto<TReturn>
                    {
                        Codigo = EstadoOperacion.Bueno,
                        Mensaje = "OK",
                        Respuesta = (TReturn)Convert.ChangeType(prestamo, typeof(TReturn))
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
