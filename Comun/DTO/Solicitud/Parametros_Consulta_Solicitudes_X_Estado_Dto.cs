using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.DTO.Solicitud
{
    public class Parametros_Consulta_Solicitudes_X_Estado_Dto
    {
        [JsonPropertyName("Id_Estado")]
        public long ID_ESTADO { get; set; }
    }
}
