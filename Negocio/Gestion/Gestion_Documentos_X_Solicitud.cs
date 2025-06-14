using Comun.DTO.Solicitud;
using Comun.Enumeracion;
using Comun.Generales;
using Datos.Contexto;
using Datos.Contratos.Solicitud;
using Datos.Modelos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Gestion
{
    public class Gestion_Documentos_X_Solicitud : IAlmacenarDocumentos
    {
        private readonly IConfiguration _configuration;
        private readonly ContextoGeneral _context;

        public Gestion_Documentos_X_Solicitud(IConfiguration configuration, ContextoGeneral context)
        {
            _configuration = configuration;
            _context = context;
        }
               

        public async Task<RespuestaDto<TReturn>> GuardarAsync<TParam, TReturn>(TParam _modelo)
        {
            try
            {
                if (_modelo is Parametros_Add_Documento_X_Solicitud_Dto parametros)
                {

                    var newDoc = new DOCUMENTOS_X_SOLICITUD
                    {
                        IdSolicitud = parametros.IdSolicitud,
                        IdDocumento = parametros.IdDocumento,
                        ContenidoDoc = parametros.ContenidoDoc,
                        Formato = parametros.Formato,
                        Tamanio = parametros.Tamanio,
                        UsuarioCreacion = parametros.UsuarioCreacion,
                        MaquinaCreacion = parametros.MaquinaCreacion,
                        Habilitado = true
                    };

                    _context.DOCUMENTOS_X_SOLICITUD.Add(newDoc);
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
                        Mensaje = "FALLO",
                        Respuesta = (TReturn)Convert.ChangeType(false, typeof(TReturn))
                    };
                }
            }
            catch (Exception ex)
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "EXCEPCION",
                    Respuesta = (TReturn)Convert.ChangeType(false, typeof(TReturn))
                };
            }
        }

        public Task<RespuestaDto<TReturn>> ObtenerAsync<TReturn>()
        {
            throw new NotImplementedException();
        }

        public Task<RespuestaDto<TReturn>> ObtenerAsync<TParam, TReturn>(TParam _modelo)
        {
            throw new NotImplementedException();
        }
        public Task<RespuestaDto<TReturn>> DeshabilitarAsync<TParam, TReturn>(TParam _identificador)
        {
            throw new NotImplementedException();
        }
    }
}
