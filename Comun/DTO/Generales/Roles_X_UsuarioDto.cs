using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Generales
{
    public class Roles_X_UsuarioDto
    {
        public decimal ID_USUARIO { get; set; }
        public decimal ID_ROL { get; set; }
        public string ROL_STR { get; set; }
        public bool HABILITADO { get; set; }
    }
}
