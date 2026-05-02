using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Comun.DTO.Solicitud
{
    public class Parametro_Codeudor_Dto
    {
        [JsonIgnore]
        public long IdSolicitud { get; set; }

        public string? PrimerNombre { get; set; }
        public string? SegundoNombre { get; set; }
        public string? PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }

        public int? TipoIdentificacion { get; set; }
        public long? NumeroIdentificacion { get; set; }

        public string? Direccion { get; set; }
        public string? Email { get; set; }
        public Int64? Celular { get; set; }


        public List<Parametros_Add_Documento_X_Solicitud_Dto> Documentos { get; set; } = new();
    }
}
