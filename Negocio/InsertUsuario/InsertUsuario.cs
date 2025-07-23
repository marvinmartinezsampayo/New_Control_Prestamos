// Ruta: Negocio/InsertUsuario/InsertUsuario.cs

using Comun.DTO.Generales;
using Comun.Enumeracion;
using Comun.Generales;
using Comun.Mapeo;
using Comun.Seguridad;
using Datos.Administracion;
using Datos.Contexto;
using Datos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Negocio.InsertUsuario
{
    public class InsertUsuario : IInsertUsuario
    {
        private readonly ContextoGeneral context;

        public InsertUsuario(ContextoGeneral _context)
        {
            context = _context;
        }

        public async Task<RespuestaDto<bool>> CrearUsuarioAsync(UsuarioDto usuarioDto)
        {
            try
            {
                if (usuarioDto == null)
                {
                    return new RespuestaDto<bool>
                    {
                        Codigo = EstadoOperacion.Malo,
                        Mensaje = "El objeto usuario es nulo.",
                        Respuesta = false
                    };
                }

                // Verifica si el usuario ya existe
                var existeusuario = await context.USUARIO.AnyAsync(u => u.USUARIO_EMPRESARIAL == usuarioDto.USUARIO_EMPRESARIAL);
                if (existeusuario)
                {
                    return new RespuestaDto<bool>
                    {
                        Codigo = EstadoOperacion.Bueno,
                        Mensaje = "El usuario ya existe.",
                        Respuesta = false
                    };
                } 
                
                var existenrousuario = await context.USUARIO.AnyAsync(u => u.NRO_IDENTIFICACION == usuarioDto.NRO_IDENTIFICACION);
                if (existenrousuario)
                {
                    return new RespuestaDto<bool>
                    {
                        Codigo = EstadoOperacion.Bueno,
                        Mensaje = "El usuario ya existe.",
                        Respuesta = false
                    };
                }

                // Hashear contraseña si existe
                if (!string.IsNullOrEmpty(usuarioDto.CONTRASENA))
                    usuarioDto.CONTRASENA = new PasswordService().HashPassword(usuarioDto.CONTRASENA);

                // Mapear DTO → Entidad EF
                USUARIO entidad = Mapeador.MapearObjeto<UsuarioDto, USUARIO>(usuarioDto);
                entidad.HABILITADO = true;

                await context.USUARIO.AddAsync(entidad);
                await context.SaveChangesAsync();

                return new RespuestaDto<bool>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "Usuario creado correctamente.",
                    Respuesta = true
                };
            }
            catch (Exception ex)
            {
                return new RespuestaDto<bool>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "Ocurrió un error al crear el usuario.",
                    Respuesta = false
                };
            }
        }
    }
}
