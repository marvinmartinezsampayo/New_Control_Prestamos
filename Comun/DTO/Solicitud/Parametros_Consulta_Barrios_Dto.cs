using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.DTO.Solicitud
{
    public class Parametros_Consulta_Barrios_Dto
    {
        [JsonPropertyName("Codigo_Dane_Municipio")]
        public long ID_DANE_MUNICIPIO { get; set; }
    }
}
