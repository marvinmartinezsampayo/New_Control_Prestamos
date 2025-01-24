using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comun.Seguridad;
using Datos.Contratos.Solicitud;
using Comun.Generales;
using Datos.Contexto;
using Microsoft.EntityFrameworkCore;
using Comun.Enumeracion;
using Comun.DTO.Solicitud;
using Google.Protobuf.WellKnownTypes;
using Comun.Mapeo;
using Datos.Modelos;

namespace Negocio.Gestion
{
    public class Gestion_Codigo_Acceso:IGenerar_Codigo
    {
        private readonly IConfiguration _configuration;
        private readonly ContextoGeneral _context;

        public Gestion_Codigo_Acceso(IConfiguration configuration, ContextoGeneral context)
        {
            _configuration = configuration;
            _context = context;
        }        

        public async Task<RespuestaDto<TReturn>> ObtenerAsync<TReturn>()
        {
            try
            {
                Int16 longitud = Convert.ToInt16(_configuration.GetSection("Codigo_Length").Value);
                string cod = "";
                string codGenerado;
                do
                {
                    codGenerado = PasswordService.GenerarCodigo(longitud);
                    var res = await _context.CODIGO_ACCESO
                                 .Where(x => x.Codigo == codGenerado)
                                 .Select(x => x.Codigo)
                                 .FirstOrDefaultAsync();
                    cod = (string)res;
                } while (cod != null);
                                
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "Operación realizada correctamente.",
                    Respuesta = (TReturn)Convert.ChangeType(codGenerado, typeof(TReturn))
                };

            }
            catch (Exception ex)
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "Se generó una excepción al ejecutar la acción.",
                    Respuesta = (TReturn)Convert.ChangeType("Ex3pt" + PasswordService.GenerarCodigo(5), typeof(TReturn))
                };               
            }
        }

        public async Task<RespuestaDto<TReturn>> ObtenerAsync<TParam, TReturn>(TParam _modelo)
        {
            try
            {
                Respuesta_Consulta_Codigo_Acceso_DTO rCod = new Respuesta_Consulta_Codigo_Acceso_DTO();

                if (_modelo is not string param || _modelo.ToString() == "" || _modelo == null)
                {
                    return new RespuestaDto<TReturn>
                    {
                        Codigo = EstadoOperacion.Malo,
                        Mensaje = "No hay datos suficientes para ejecutar la acción."
                    };
                }

                var cantidad = _context.SOLICITUD_PRESTAMO
                                .Where(x=>x.Codigo_Acceso == _modelo.ToString())
                                .Count();
                
                var resCod = await _context.CODIGO_ACCESO
                                 .Where(x => x.Codigo == _modelo.ToString() && x.Habilitado)                                 
                                 .FirstOrDefaultAsync();
                //.Where(x => x.Codigo == _modelo.ToString() && x.Habilitado && x.FechaFin >= DateTime.Now)
                //x.FechaFin < DateTime.Now && x.CantidadRegistros <= cantidad
                if (resCod != null && cantidad <= resCod.CantidadRegistros)
                {
                    rCod = Mapeador.MapearObjeto<CODIGO_ACCESO, Respuesta_Consulta_Codigo_Acceso_DTO>(resCod);
                }               


                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "Operación realizada correctamente.",
                    Respuesta = (TReturn)Convert.ChangeType(rCod, typeof(TReturn))
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
    }
}
