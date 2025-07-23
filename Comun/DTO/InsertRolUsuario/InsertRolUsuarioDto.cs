using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.InsertRolUsuario
{
    public class InsertRolUsuarioDto
    {
        public Int64 Id { get; set; }
        public long Id_Usuario { get; set; }
        public int  Id_Rol {  get; set; }
        public bool Habilitado { get; set; }
    }
}
