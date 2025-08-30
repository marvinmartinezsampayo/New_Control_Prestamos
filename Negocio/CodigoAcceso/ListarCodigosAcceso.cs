using Comun.DTO.CodigoAcceso;
using Comun.Enumeracion;
using Comun.Generales;
using Datos.CodigoAcceso;
using Datos.Contexto;
using Datos.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.CodigoAcceso
{
    public class ListarCodigosAcceso : IListarCodigosAcceso

    {

        private readonly IConfiguration _configuration;
        private readonly ContextoGeneral _context;


        public ListarCodigosAcceso(IConfiguration configuration, ContextoGeneral context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<RespuestaDto<TReturn>> ObtenerCodigosDeHoyGenerico<TReturn>()
        {
            try
            {
                var hoy = DateTime.Today;

                var codigos = await _context.CODIGO_ACCESO
                    .Where(c => c.Habilitado && c.FechaInicio.Date == hoy)
                    .Select(c => new Codigo_AccesoDto
                    {
                        Codigo = c.Codigo,
                        FechaInicio = c.FechaInicio,
                        FechaFin = c.FechaFin,
                        CantidadRegistros = (int)c.CantidadRegistros,
                        UsuarioCreacion = c.UsuarioCreacion,
                        Habilitado = c.Habilitado
                    })
                    .ToListAsync();

                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "Consulta realizada correctamente.",
                    Respuesta = (TReturn)Convert.ChangeType(codigos, typeof(TReturn))
                };
            }
            catch (Exception ex)
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = $"Se generó una excepción al ejecutar la acción. {ex.Message}",
                    Respuesta = default
                };
            }
        }

        public async Task<RespuestaDto<TReturn>> ObtenerCodigosPorFechasAsync<TReturn>(DateTime fechaInicio, DateTime fechaFin, string Codigo)
        {
            try
            {
                IQueryable<CODIGO_ACCESO> query = _context.CODIGO_ACCESO;

                // Validar si vienen parámetros útiles
                bool vieneCodigo = !string.IsNullOrWhiteSpace(Codigo);
                bool vieneFechas = fechaInicio != DateTime.MinValue && fechaFin != DateTime.MinValue;

                // Si viene código, filtra por código
                if (vieneCodigo)
                {
                    query = query.Where(c => c.Codigo.Contains(Codigo));
                }
                // Si no viene código, pero sí fechas válidas, filtra por fechas
                else if (vieneFechas)
                {
                    query = query.Where(c => c.FechaInicio >= fechaInicio && c.FechaFin <= fechaFin);
                }
                // Si no viene ni código ni fechas válidas, se retorna advertencia
                else
                {
                    return new RespuestaDto<TReturn>
                    {
                        Codigo = EstadoOperacion.Excepcion,
                        Mensaje = "Debe ingresar al menos un filtro (código o fechas).",
                        Respuesta = default
                    };
                }

                // Ejecutar la consulta y transformar resultados
                var codigos = await query
                    .Select(c => new Codigo_AccesoDto
                    {
                        Codigo = c.Codigo,
                        FechaInicio = c.FechaInicio,
                        FechaFin = c.FechaFin,
                        CantidadRegistros = (int)c.CantidadRegistros,
                        UsuarioCreacion = c.UsuarioCreacion,
                        Habilitado = c.Habilitado
                    })
                    .ToListAsync();

                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "Consulta realizada correctamente.",
                    Respuesta = (TReturn)Convert.ChangeType(codigos, typeof(TReturn))
                };
            }
            catch (Exception ex)
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = $"Se generó una excepción al ejecutar la acción. {ex.Message}",
                    Respuesta = default
                };
            }
        }


        public async Task<RespuestaDto<string>> ActualizarCodigoAccesoAsync(UpdateCodigoosAcccesoDto dto)
        {
            try
            {
                var codigoAcceso = await _context.CODIGO_ACCESO
                    .FirstOrDefaultAsync(c => c.Codigo == dto.Codigo);

                if (codigoAcceso == null)
                {
                    return new RespuestaDto<string>
                    {
                        Codigo = EstadoOperacion.Excepcion,
                        Mensaje = "Código de acceso no encontrado.",
                        Respuesta = null
                    };
                }

                
                if (dto.CantidadRegistros.HasValue)
                {
                    // Contar cuántas solicitudes ya usan este código
                    int totalUsosCodigo = await _context.SOLICITUD_PRESTAMO
                        .CountAsync(s => s.CodigoAcceso == dto.Codigo);

                    if (dto.CantidadRegistros.Value < totalUsosCodigo)
                    {
                        return new RespuestaDto<string>
                        {
                            Codigo = EstadoOperacion.Excepcion,
                            Mensaje = $"No se puede establecer la cantidad en {dto.CantidadRegistros.Value}, ya existen {totalUsosCodigo} solicitudes con este código.",
                            Respuesta = null
                        };
                    }

                    // Solo si pasa la validación, actualizamos
                    codigoAcceso.CantidadRegistros = dto.CantidadRegistros.Value;
                }

                if (dto.FechaInicio.HasValue)
                    codigoAcceso.FechaInicio = dto.FechaInicio.Value;

                if (dto.FechaFin.HasValue)
                    codigoAcceso.FechaFin = dto.FechaFin.Value;

                if (dto.Habilitado.HasValue)
                    codigoAcceso.Habilitado = dto.Habilitado.Value;

                await _context.SaveChangesAsync();

                return new RespuestaDto<string>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "Código de acceso actualizado correctamente.",
                    Respuesta = codigoAcceso.Codigo
                };
            }
            catch (Exception ex)
            {
                return new RespuestaDto<string>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = $"Se generó un error al actualizar: {ex.Message}",
                    Respuesta = null
                };
            }
        }



    }
}
