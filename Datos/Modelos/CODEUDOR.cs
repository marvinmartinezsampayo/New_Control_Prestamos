using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Modelos
{
    [Table("codeudor")]
    public class CODEUDOR
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }

        [Column("ID_SOLICITUD")]
        [Required]
        public long IdSolicitud { get; set; }

        [Column("PRIMER_NOMBRE")]
        public string PNombre { get; set; }

        [Column("SEGUNDO_NOMBRE")]
        public string? SNombre { get; set; }

        [Column("PRIMER_APELLIDO")]
        public string PApellido { get; set; }

        [Column("SEGUNDO_APELLIDO")]
        public string? SApellido { get; set; }

        [Column("ID_TIPO_IDENTIFICACION")]
        public long TipoIdentificacion { get; set; }

        [Column("NRO_IDENTIFICACION")]
        [Required]
        public long NumeroIdentificacion { get; set; }

        [Column("DIRECCION")]
        [Required]
        public string Direccion { get; set; }

        [Column("EMAIL")]
        [Required]
        public string Email { get; set; }

        [Column("CELULAR")]
        public long? Celular { get; set; }

        // RELACIÓN
        [ForeignKey("IdSolicitud")]
        public SOLICITUD_PRESTAMO? Solicitud { get; set; }
    }

}
