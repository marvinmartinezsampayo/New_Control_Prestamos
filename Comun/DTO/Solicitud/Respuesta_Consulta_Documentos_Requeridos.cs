using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.DTO.Solicitud
{
    public class Respuesta_Consulta_Documentos_Requeridos
    {
        [JsonPropertyName("Id")]
        public long Id { get; set; }

        [JsonPropertyName("Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("Descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [JsonPropertyName("PesoMaximo")]
        public long PesoMaximo { get; set; }
    }
}
