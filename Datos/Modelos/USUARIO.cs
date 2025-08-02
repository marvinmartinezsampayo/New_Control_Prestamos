using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Modelos
{
    [Serializable]
    [Table(nameof(USUARIO))]
    public class USUARIO
    {
        [Key]
        [Column("ID")]
        public long ID { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("USUARIO_EMPRESARIAL")]
        public string USUARIO_EMPRESARIAL { get; set; }

        [Column("NRO_IDENTIFICACION")]
        public long NRO_IDENTIFICACION { get; set; }

        [ForeignKey("ID_TIPO")]
        [Column("ID_TIPO_IDENTIFICACION")]
        public long ID_TIPO_IDENTIFICACION { get; set; }

        [MaxLength(50)]
        [Column("PRIMER_NOMBRE")]
        public string PRIMER_NOMBRE { get; set; }

        [MaxLength(50)]
        [Column("SEGUNDO_NOMBRE")]
        public string SEGUNDO_NOMBRE { get; set; }

        [MaxLength(50)]
        [Column("PRIMER_APELLIDO")]
        public string PRIMER_APELLIDO { get; set; }

        [MaxLength(50)]
        [Column("SEGUNDO_APELLIDO")]
        public string SEGUNDO_APELLIDO { get; set; }

        [MaxLength(50)]
        [Column("EMAIL")]
        public string EMAIL { get; set; }

        [MaxLength(50)]
        [Column("TELEFONO")]
        public string TELEFONO { get; set; }

        [MaxLength(200)]
        [Column("CONTRASENA")]
        public string CONTRASENA { get; set; }

        [Column("HABILITADO")]
        public bool HABILITADO { get; set; }

        [Column("FOTO")]
        public byte[]? FOTO { get; set; }

        public virtual DETALLE_MASTER ID_TIPO { get; set; }
    }
}
