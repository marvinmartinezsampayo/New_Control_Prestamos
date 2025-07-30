using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.CodigoAcceso
{
    public class UpdateCodigoosAcccesoDto
    {
        public string Codigo { get; set; } // requerido para identificar el registro
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? CantidadRegistros { get; set; }
        public bool? Habilitado { get; set; }
        public string UsuarioModificacion { get; set; }
        public Int64 Id_Usuario { get; set; }
    }
}
