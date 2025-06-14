using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Solicitud
{
    public class Parametros_Add_Documento_X_Solicitud_Dto
    {
        [Required(ErrorMessage = "El ID de solicitud es requerido")]
        public long IdSolicitud { get; set; }

        [Required(ErrorMessage = "El ID de documento es requerido")]
        public long IdDocumento { get; set; }

        [Required(ErrorMessage = "El contenido del documento es requerido")]
        public string ContenidoDoc { get; set; }

        [Required(ErrorMessage = "El formato es requerido")]
        [MaxLength(50, ErrorMessage = "El formato no puede exceder 50 caracteres")]
        public string Formato { get; set; }

        [Required(ErrorMessage = "El tamaño es requerido")]
        [Range(1, long.MaxValue, ErrorMessage = "El tamaño debe ser mayor a 0")]
        public long Tamanio { get; set; }

        [Required(ErrorMessage = "El usuario de creación es requerido")]
        [MaxLength(100, ErrorMessage = "El usuario de creación no puede exceder 100 caracteres")]
        public string UsuarioCreacion { get; set; }

        [Required(ErrorMessage = "La máquina de creación es requerida")]
        [MaxLength(100, ErrorMessage = "La máquina de creación no puede exceder 100 caracteres")]
        public string MaquinaCreacion { get; set; }

        [Required(ErrorMessage = "El estado habilitado es requerido")]
        public bool Habilitado { get; set; } = true;
    }
}
