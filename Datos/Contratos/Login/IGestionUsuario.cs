using Comun.DTO.Generales;
using Comun.DTO.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Contratos.Login
{
    public interface IGestionUsuario
    {
        Task<UsuarioDto> ValidarLogin(LoginDto _user);

        Task<List<Roles_X_UsuarioDto>>  ConsultarRolesUsuario(decimal _idUsuario);

        Task<List<Detalle_MasterDto>>  ConsultarDetalleMaster(decimal _idTipoDetalle);
    }
}
