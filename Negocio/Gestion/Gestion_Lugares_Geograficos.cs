using Comun.DTO.Solicitud;
using Comun.Enumeracion;
using Comun.Generales;
using Comun.Mapeo;
using Datos.Contexto;
using Datos.Contratos.Solicitud;
using Datos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Gestion
{
    public class Gestion_Lugares_Geograficos : ILugaresGeograficos
    {
        private readonly ContextoGeneral context;        

        public Gestion_Lugares_Geograficos(ContextoGeneral _context)
        {
            context = _context;
        }

        public Task<RespuestaDto<TReturn>> ObtenerAsync<TReturn>()
        {
            throw new NotImplementedException();
        }

        public async Task<RespuestaDto<TReturn>> ObtenerAsync<TParam, TReturn>(TParam _modelo)
        {
            if (_modelo is not Parametros_Consulta_Lugares_Geograficos_DTO param)
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Malo,
                    Mensaje = "No hay datos suficientes para ejecutar la acción."
                };
            }

            try
            {
                IQueryable<LUGARES_GEOGRAFICOS> query = context.LUGARES_GEOGRAFICOS
                                                        .Where(x => x.TipoLugar == param.TIPO_LUGAR && x.Habilitado);

                if (param.ID_DANE_PADRE.HasValue && param.ID_DANE_PADRE > 0)
                {
                    query = query.Where(x => x.IdDanePadre == param.ID_DANE_PADRE);
                }

                var lstLugares = await query.ToListAsync();

                var resp = lstLugares.Select(lugar =>
                    Mapeador.MapearObjeto<LUGARES_GEOGRAFICOS, Respuesta_Consulta_Lugares_Geograficos_DTO>(lugar))
                    .ToList();

                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "Operación realizada correctamente.",
                    Respuesta = (TReturn)Convert.ChangeType(resp, typeof(TReturn))
                };
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "Se generó una excepción al ejecutar la acción."
                };
            }
        }

        public async Task<RespuestaDto<TReturn>> ObtenerBarriosAsync<TParam, TReturn>(TParam _modelo)
        {
            
            if (_modelo is not Parametros_Consulta_Barrios_Dto param)
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Malo,
                    Mensaje = "No hay datos suficientes para ejecutar la acción."
                };
            }

            try
            {
                IQueryable<BARRIOS> query = context.BARRIOS
                                            .Where(x => x.CodigoDaneMpio == param.ID_DANE_MUNICIPIO && x.Habilitado);                
                var lstLugares = await query.ToListAsync();

                var resp = lstLugares.Select(lugar =>
                    Mapeador.MapearObjeto<BARRIOS, Respuesta_Consulta_Barrios_Dto>(lugar))
                    .ToList();

                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "Operación realizada correctamente.",
                    Respuesta = (TReturn)Convert.ChangeType(resp, typeof(TReturn))
                };
            }
            catch (Exception ex)
            {                
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "Se generó una excepción al ejecutar la acción."
                };
            }
        }

        public async Task<RespuestaDto<bool>> ActualizarEstadoSolicitudAsync<TParam>(TParam _modelo)
        {
            if (_modelo is not Parametros_Actualizar_Estado_Solicitud_Dto param)
            {
                return new RespuestaDto<bool>
                {
                    Codigo = EstadoOperacion.Malo,
                    Mensaje = "No hay datos suficientes para ejecutar la acción.",
                    Respuesta = false
                };
            }

            try
            {
                var solicitud = await context.SOLICITUD_PRESTAMO.FirstOrDefaultAsync(x => x.Id == param.IdSolicitud);

                if (solicitud == null)
                {
                    return new RespuestaDto<bool>
                    {
                        Codigo = EstadoOperacion.Malo,
                        Mensaje = "No se encontró la solicitud especificada.",
                        Respuesta = false
                    };
                }

                solicitud.EstadoId = param.NuevoEstadoId;
                await context.SaveChangesAsync();

                return new RespuestaDto<bool>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "Estado actualizado correctamente.",
                    Respuesta = true
                };
            }
            catch (Exception ex)
            {
                return new RespuestaDto<bool>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "Ocurrió un error al actualizar el estado.",
                    Respuesta = false
                };
            }
        }

    }
}
