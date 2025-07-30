using Comun.DTO.CodigoAcceso;
using Comun.Enumeracion;
using Comun.Generales;
using Datos.CodigoAcceso;
using Datos.Contexto;
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

        public async Task<RespuestaDto<TReturn>> ObtenerCodigosPorFechasAsync<TReturn>(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var codigos = await _context.CODIGO_ACCESO
                 .Where(c =>
                     c.FechaInicio >= fechaInicio &&
                     c.FechaFin <= fechaFin)
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

                // Solo actualiza si el campo viene con valor
                if (dto.FechaInicio.HasValue)
                    codigoAcceso.FechaInicio = dto.FechaInicio.Value;

                if (dto.FechaFin.HasValue)
                    codigoAcceso.FechaFin = dto.FechaFin.Value;

                if (dto.CantidadRegistros.HasValue)
                    codigoAcceso.CantidadRegistros = dto.CantidadRegistros.Value;

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
