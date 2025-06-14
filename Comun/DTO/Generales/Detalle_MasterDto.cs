using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Generales
{
    public class Detalle_MasterDto
    {  
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Abreviacion { get; set; }
        public string Descripcion { get; set; }
        public long? IdTipo { get; set; }
        public bool Habilitado { get; set; }
    }
}
