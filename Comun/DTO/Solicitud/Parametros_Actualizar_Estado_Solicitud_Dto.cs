using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Solicitud
{
    public class Parametros_Actualizar_Estado_Solicitud_Dto
    {
        public long IdSolicitud { get; set; }
        public int NuevoEstadoId { get; set; }
    }
}
