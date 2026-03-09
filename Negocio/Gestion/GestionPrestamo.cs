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
        private readonly ContextoLocal _context;

        public GestionPrestamo(IConfiguration configuration, ContextoLocal context)
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
                        ID_TIPO_PAGO = pago.ID_TIPO_PAGO,
                        NUMERO_CUOTA = (int?)pago.NUMERO_CUOTA
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

        public async Task<RespuestaDto<TReturn>> Actualizar_Prestamo_Async<TParam, TReturn>(TParam _modelo)
        {
            try
            {
                if (_modelo is ActualizarPrestamoDto pago)
                {
                    var prestamo = await _context.PRESTAMOS.FirstOrDefaultAsync(p => p.ID == pago.ID);
                    
                    if (prestamo != null)
                    {
                        prestamo.SALDO_MONTO = (long)pago.SALDO;

                        if (pago.ID_ESTADO.HasValue && pago.ID_ESTADO.Value > 0)
                        {
                            prestamo.ID_ESTADO = (long)pago.ID_ESTADO;
                        }
                        
                        await _context.SaveChangesAsync();

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
                            Mensaje = "No de encuentra el prestamo seleccionado.",
                            Respuesta = (TReturn)Convert.ChangeType(false, typeof(TReturn))
                        };
                    }

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
                            SALDO_PENDIENTE = p.MONTO - _context.PAGOS
                                .Where(pg => pg.ID_PRESTAMO == p.ID)
                                .Sum(pg => pg.MONTO)
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

        public async Task<RespuestaDto<TReturn>> Obtener_Pagos_Async<TParam, TReturn>(TParam _modelo)
        {
            try
            {
                List<Pago_Dto> resp = new List<Pago_Dto>();

                if (_modelo is Int64 IdPrestamo)
                {
                    var listPagos = await _context.PAGOS
                                        .Include(p => p.FK_ID_PRESTAMO)
                                        .Include(p => p.FK_ID_TIPO_PAGO)
                                        .Where(p => p.ID_PRESTAMO == IdPrestamo)
                                        .Select(p => new Pago_Dto
                                        {
                                            ID = p.ID,
                                            ID_PRESTAMO = IdPrestamo,
                                            FECHA_PAGO = p.FECHA_PAGO,
                                            MONTO = p.MONTO,
                                            NUMERO_CUOTA = p.NUMERO_CUOTA,
                                            ID_TIPO_PAGO = p.ID_TIPO_PAGO,
                                            TIPO_PAGO = p.FK_ID_TIPO_PAGO.Descripcion
                                        })
                                        .OrderBy(p => p.FECHA_PAGO)
                                        .ToListAsync();

                    return new RespuestaDto<TReturn>
                    {
                        Codigo = EstadoOperacion.Bueno,
                        Mensaje = "OK",
                        Respuesta = (TReturn)Convert.ChangeType(listPagos, typeof(TReturn))
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

        public async Task<RespuestaDto<TReturn>> Obtener_All_Prestamos_Async<TParam, TReturn>(TParam _modelo)
        {
            try
            {
                var prestamos = await (
                    from pr in _context.PRESTAMOS
                    join sol in _context.SOLICITUD_PRESTAMO on pr.ID_SOLICITUD equals sol.Id
                    join dm in _context.DETALLE_MASTER on pr.ID_ESTADO equals dm.Id
                    select new PrestamoResumenDto
                    {
                        ID_PRESTAMO = pr.ID,
                        ID_SOLICITUD = sol.Id,
                        NUMERO_IDENTIFICACION = sol.NumeroIdentificacion,
                        P_NOMBRE_SOLICITANTE = sol.PrimerNombreSolicitante,
                        S_NOMBRE_SOLICITANTE = sol.SegundoNombreSolicitante,
                        P_APELLIDO_SOLICITANTE = sol.PrimerApellidoSolicitante,
                        S_APELLIDO_SOLICITANTE = sol.SegundoApellidoSolicitante,
                        POR_INTERES = pr.INTERES,
                        CATEGORIA = "A",
                        MONTO = pr.MONTO,
                        MULTAS = 0,
                        SALDO_MORA = 0,
                        RETORNO = 0,
                        RETORNO_MES = (long)Math.Round((double)(pr.MONTO / pr.NUMERO_CUOTAS), 0),
                        INTERES = 0,
                        INTERES_MES = (long)Math.Round((double)(pr.SALDO_MONTO * pr.INTERES) / 100, 0),
                        SALDO_PENDIENTE = pr.SALDO_MONTO,
                        INTERES_A_COBRAR = 0,
                        ID_ESTADO = pr.ID_ESTADO,
                        ESTADO_PRESTAMO = dm.Nombre
                    }
                ).ToListAsync();

                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "OK",
                    Respuesta = (TReturn)(object)prestamos
                };
            }
            catch (Exception ex)
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Malo,
                    Mensaje = ex.Message
                };
            }
        }

        public async Task<RespuestaDto<TReturn>> Obtener_Sumatoria_Depositos_Inversores_Async<TReturn>()
        {
            try
            {
                const long ESTADO_ACTIVO = 58;

                decimal sumatoria = await _context.DEPOSITO_INVERSOR
                    .Where(d => d.IdEstado == ESTADO_ACTIVO)
                    .SumAsync(d => d.ValorDepositado);

                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "OK",
                    Respuesta = (TReturn)(object)sumatoria
                };
            }
            catch (Exception ex)
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Malo,
                    Mensaje = ex.Message
                };
            }
        }

        public async Task<RespuestaDto<TReturn>> Obtener_Sumatoria_Saldos_Creditos_Async<TReturn>()
        {
            try
            {
                const long ESTADO_CREDITO_ACTIVO = 51;

                decimal sumatoria = await _context.PRESTAMOS
                    .Where(p => p.ID_ESTADO == ESTADO_CREDITO_ACTIVO)
                    .SumAsync(p => p.SALDO_MONTO);

                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "OK",
                    Respuesta = (TReturn)(object)sumatoria
                };
            }
            catch (Exception ex)
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Malo,
                    Mensaje = ex.Message
                };
            }
        }

        public async Task<RespuestaDto<TReturn>> Obtener_Sumatoria_Monto_Pagos_Async<TParam, TReturn>(TParam _modelo)
        {
            try
            {
                long TIPO_PAGO = 0;
                Type tipo = Nullable.GetUnderlyingType(typeof(TParam)) ?? typeof(TParam);

                if (tipo == typeof(long))
                {
                    TIPO_PAGO = Convert.ToInt64(_modelo);
                }

                decimal sumatoria = await _context.PAGOS
                    .Where(p => p.ID_TIPO_PAGO == TIPO_PAGO)
                    .SumAsync(p => p.MONTO);

                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "OK",
                    Respuesta = (TReturn)(object)sumatoria
                };
            }
            catch (Exception ex)
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Malo,
                    Mensaje = ex.Message
                };
            }
        }
    }
}
