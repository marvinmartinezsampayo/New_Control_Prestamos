using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Solicitud
{
    public class Respuesta_Consulta_Solicitudes_Prestamo_Dto
    {
        public long Id { get; set; }
        public string NombreCompleto => $"{PrimerNombreSolicitante} {SegundoNombreSolicitante} {PrimerApellidoSolicitante} {SegundoApellidoSolicitante}".Trim();
        public string PrimerNombreSolicitante { get; set; }
        public string SegundoNombreSolicitante { get; set; }
        public string PrimerApellidoSolicitante { get; set; }
        public string SegundoApellidoSolicitante { get; set; }
        public long TipoIdentificacionId { get; set; }
        public long NumeroIdentificacion { get; set; }
        public long DepartamentoResidenciaId { get; set; }
        public long MunicipioResidenciaId { get; set; }
        public long BarrioResidenciaId { get; set; }
        public string DireccionResidencia { get; set; }
        public long EstadoId { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public long Monto { get; set; }
        public string CodigoAcceso { get; set; }
        public bool Habilitado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<Parametros_Add_Documento_X_Solicitud_Dto>? Documentos { get; set; }
    }
}
