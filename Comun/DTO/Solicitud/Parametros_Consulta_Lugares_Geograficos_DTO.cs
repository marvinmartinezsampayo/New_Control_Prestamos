using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.DTO.Solicitud
{
    public class Parametros_Consulta_Lugares_Geograficos_DTO
    {
        [JsonPropertyName("Tipo_Lugar")]
        public string TIPO_LUGAR { get; set; }

        [JsonPropertyName("Codigo_Dane_Padre")]
        public long? ID_DANE_PADRE { get; set; }
    }
}
