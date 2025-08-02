using Comun.DTO.BuscarUsuario;
using Comun.DTO.Generales;
using Comun.Enumeracion;
using Comun.Generales;
using Comun.Mapeo;
using Datos.Contexto;
using Datos.Contratos.Usuario;
using Datos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.BuscarUsuario
{
    public class BuscarUsuarioNurIdentificacion : IBusacrUsuarioNurIdentificacion
    {
        private readonly ContextoGeneral context;

        public BuscarUsuarioNurIdentificacion(ContextoGeneral _context)
        {
            context = _context;
        }


public async Task<RespuestaDto<TReturn>> ObtenerUsuarioPorIdentificacionAsync<TParam, TReturn>(TParam _modelo)
    {
        if (_modelo is not BuscarUsuarioNurIdentificacionPet param)
        {
            return new RespuestaDto<TReturn>
            {
                Codigo = EstadoOperacion.Malo,
                Mensaje = "No se enviaron parámetros válidos."
            };
        }

        try
        {
            var entidad = await context.USUARIO
                .FirstOrDefaultAsync(u => u.NRO_IDENTIFICACION == param.NRO_IDENTIFICACION);

            if (entidad == null)
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Malo,
                    Mensaje = "No se encontró el usuario con esa identificación."
                };
            }

            var dto = Mapeador.MapearObjeto<USUARIO, UsuarioDto>(entidad);

            return new RespuestaDto<TReturn>
            {
                Codigo = EstadoOperacion.Bueno,
                Mensaje = "Usuario encontrado correctamente.",
                Respuesta = (TReturn)Convert.ChangeType(dto, typeof(TReturn))
            };
        }
        catch (Exception ex)
        {
            return new RespuestaDto<TReturn>
            {
                Codigo = EstadoOperacion.Excepcion,
                Mensaje = $"Error al consultar el usuario: {ex.Message}"
            };
        }
    }


}
}
