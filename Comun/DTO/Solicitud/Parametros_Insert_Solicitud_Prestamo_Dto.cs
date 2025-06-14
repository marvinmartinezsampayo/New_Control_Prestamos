using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Comun.DTO.Solicitud
{
    public class Parametros_Insert_Solicitud_Prestamo_Dto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("p_nombre_solicitante")]
        public string PNombreSolicitante { get; set; } = string.Empty;

        [JsonPropertyName("s_nombre_solicitante")]
        public string? SNombreSolicitante { get; set; }

        [JsonPropertyName("p_apellido_solicitante")]
        public string PApellidoSolicitante { get; set; } = string.Empty;

        [JsonPropertyName("s_apellido_solicitante")]
        public string? SApellidoSolicitante { get; set; }

        [JsonPropertyName("tipo_identificacion")]
        public long TipoIdentificacion { get; set; }

        [JsonPropertyName("numero_identificacion")]
        public long NumeroIdentificacion { get; set; }

        [JsonPropertyName("id_depto_residencia")]
        public long IdDeptoResidencia { get; set; }

        [JsonPropertyName("id_mpio_residencia")]
        public long IdMpioResidencia { get; set; }

        [JsonPropertyName("id_barrio_residencia")]
        public long IdBarrioResidencia { get; set; }

        [JsonPropertyName("direccion_residencia")]
        public string DireccionResidencia { get; set; } = string.Empty;

        [JsonPropertyName("id_genero")]
        public long IdGenero { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("celular")]
        public string Celular { get; set; } = string.Empty;

        [JsonPropertyName("codigo_acceso")]
        public string? CodigoAcceso { get; set; }

        [JsonPropertyName("habilitado")]
        public byte Habilitado { get; set; }

        [JsonPropertyName("usuario_creacion")]
        public string UsuarioCreacion { get; set; } = string.Empty;

        [JsonPropertyName("maquina_creacion")]
        public string MaquinaCreacion { get; set; } = string.Empty;

        [JsonPropertyName("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [JsonPropertyName("lista_documentos")]
        public List<Parametros_Add_Documento_X_Solicitud_Dto>? Documentos { get; set; }
    }
}
