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

            [Column("P_NOMBRE")]
            [Required]
            [MaxLength(20)]
            public string PNombre { get; set; }

            [Column("S_NOMBRE")]
            [MaxLength(20)]
            public string SNombre { get; set; }

            [Column("P_APELLIDO")]
            [Required]
            [MaxLength(20)]
            public string PApellido { get; set; }

            [Column("S_APELLIDO")]
            [MaxLength(20)]
            public string SApellido { get; set; }

            [Column("TIPO_IDENTIFICACION")]
            [Required]
            public long TipoIdentificacion { get; set; }

            [Column("NUMERO_IDENTIFICACION")]
            [Required]
            public long NumeroIdentificacion { get; set; }

            [Column("DIRECCION")]
            [Required]
            [MaxLength(400)]
            public string Direccion { get; set; }

            [Column("EMAIL")]
            [Required]
            [MaxLength(50)]
            public string Email { get; set; }

            [Column("CELULAR")]
            [Required]
            [MaxLength(50)]
            public string Celular { get; set; }

            [Column("HABILITADO")]
            [Required]
            public bool Habilitado { get; set; } = true;

            [Column("USUARIO_CREACION")]
            [MaxLength(100)]
            public string UsuarioCreacion { get; set; }

            [Column("MAQUINA_CREACION")]
            [MaxLength(100)]
            public string MaquinaCreacion { get; set; }

            [Column("FECHA_CREACION")]
            [Required]
            public DateTime FechaCreacion { get; set; }

            // RELACIÓN
            [ForeignKey("IdSolicitud")]
            public SOLICITUD_PRESTAMO Solicitud { get; set; }
        }
    
}
