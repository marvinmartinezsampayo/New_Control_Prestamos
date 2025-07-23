using Comun.DTO.InsertRolUsuario;
using Comun.Generales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Administracion
{
    public interface IInsertRolUsuario
    {
        Task<RespuestaDto<bool>> InsertarRolUsuarioAsync(InsertRolUsuarioDto dto);
    }
}
