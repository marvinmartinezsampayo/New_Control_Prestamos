using Comun.DTO.Generales;
using Comun.Generales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Administracion
{
    public interface IInsertUsuario
    {
        Task<RespuestaDto<bool>> CrearUsuarioAsync(UsuarioDto usuarioDto);
    }
}
