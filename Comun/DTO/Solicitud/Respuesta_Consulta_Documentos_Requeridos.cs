using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Solicitud
{
    public class Respuesta_Consulta_Documentos_Requeridos
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public long PesoMaximo { get; set; }
    }
}
