using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Generales
{
    public class Roles_X_UsuarioDto
    {
        public long ID_USUARIO { get; set; }
        public long ID_ROL { get; set; }
        public string ROL_STR { get; set; }
        public string ROL_DESCRIPCION { get; set; }
        public bool HABILITADO { get; set; }
    }

}
