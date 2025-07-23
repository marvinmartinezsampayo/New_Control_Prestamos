using Comun.DTO.Generales;
using Comun.DTO.Seguridad;
using Comun.Mapeo;
using Comun.Seguridad;
using Datos.Contexto;
using Datos.Contratos.Login;
using Datos.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Negocio.Implementacion
{
    public class Gestion_Login : IGestionUsuario
    {
        private readonly ContextoGeneral context;

        public Gestion_Login(ContextoGeneral _context)
        {
            context = _context;
        }


        public Task<List<Detalle_MasterDto>> ConsultarDetalleMaster(decimal _idTipoDetalle)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Roles_X_UsuarioDto>> ConsultarRolesUsuario(decimal _idUsuario)
        {
            try
            {
                if (_idUsuario <= 0)
                    return new List<Roles_X_UsuarioDto>();

                var user = await context.ROLES_X_USUARIO
                                .Include(x => x.FK_ID_ROL) 
                                .Where(x => x.ID_USUARIO == _idUsuario && x.HABILITADO)
                                .Select(u => new Roles_X_UsuarioDto
                              {
                                  ID_USUARIO = u.ID_USUARIO,
                                  ID_ROL = u.ID_ROL,
                                  ROL_STR = u.FK_ID_ROL.Nombre,
                                  ROL_DESCRIPCION = u.FK_ID_ROL.Descripcion,
                                  HABILITADO = u.HABILITADO
                              })
                              .ToListAsync();

                return user;
            }
            catch (Exception)
            {
                return new List<Roles_X_UsuarioDto>();
            }
        }

        public async Task<UsuarioDto> ValidarLogin(LoginDto _user)
        {
            try
            {
                if (string.IsNullOrEmpty(_user.Usuario) || string.IsNullOrEmpty(_user.Clave))
                    return new UsuarioDto() { };

                var user = await context.USUARIO
                            .Where(u => u.USUARIO_EMPRESARIAL.ToUpper() == _user.Usuario.ToUpper() && u.HABILITADO == true)
                            .Select(u => new { u.ID, u.CONTRASENA })
                            .FirstOrDefaultAsync();

                if (user == null)
                    return new UsuarioDto() { };
                string ps = new PasswordService().HashPassword(_user.Clave);

                //string nuevoHash = BCrypt.Net.BCrypt.HashPassword("Colombia*2024*+");
                //Console.WriteLine(nuevoHash); // Copia este valor a la BD.

                bool validacion = VerifyPassword(_user.Clave, user.CONTRASENA);

                if (validacion)
                {
                    try
                    {
                        var respUser = await context.USUARIO.Where(u => u.ID == user.ID).FirstOrDefaultAsync();
                        UsuarioDto RESP = Mapeador.MapearObjetoSeguro<USUARIO, UsuarioDto>(respUser);
                        return RESP;
                    }
                    catch (Exception ex)
                    {
                        return new UsuarioDto() { };
                    }
                }
                else
                {
                    return new UsuarioDto() { };
                }
            }
            catch (Exception)
            {
                return new UsuarioDto() { };
            }
        }

        // Verificar una contraseña
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
