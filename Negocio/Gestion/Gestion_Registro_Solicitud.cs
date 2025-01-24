using Comun.Enumeracion;
using Comun.Generales;
using Comun.Seguridad;
using Datos.Contexto;
using Datos.Contratos.Solicitud;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Gestion
{
    public class Gestion_Registro_Solicitud : IRegistroSolicitud
    {
        private readonly IConfiguration _configuration;
        private readonly ContextoGeneral _context;

        public Gestion_Registro_Solicitud(IConfiguration configuration, ContextoGeneral context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<RespuestaDto<TReturn>> ContarAsync<TParam, TReturn>(TParam param)
        {
            try
            {
                long cantidad = _context.SOLICITUD_PRESTAMO
                               .Where(x => x.Codigo_Acceso == param.ToString())
                               .Count();

                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "Operación realizada correctamente.",
                    Respuesta = (TReturn)Convert.ChangeType(cantidad, typeof(TReturn))
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

        public Task<RespuestaDto<TReturn>> DeshabilitarAsync<TParam, TReturn>(TParam _identificador)
        {
            throw new NotImplementedException();
        }

        public Task<RespuestaDto<TReturn>> GuardarAsync<TParam, TReturn>(TParam _modelo)
        {
            throw new NotImplementedException();
        }

        public Task<RespuestaDto<TReturn>> HabilitarAsync<TParam, TReturn>(TParam _identificador)
        {
            throw new NotImplementedException();
        }

        public Task<RespuestaDto<TReturn>> ObtenerAsync<TReturn>()
        {
            throw new NotImplementedException();
        }

        public Task<RespuestaDto<TReturn>> ObtenerAsync<TParam, TReturn>(TParam _modelo)
        {
            throw new NotImplementedException();
        }
    }
}
