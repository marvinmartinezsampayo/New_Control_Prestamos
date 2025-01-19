using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Modelos
{
    [Table("ROLES_X_USUARIO")]
    public class ROLES_X_USUARIO
    {
        [Key]
        [Column("ID")]
        public long ID { get; set; }

        [Required]
        [ForeignKey("FK_ID_USUARIO")]
        [Column("ID_USUARIO")]
        public long ID_USUARIO { get; set; }

        [Required]
        [ForeignKey("FK_ID_ROL")]
        [Column("ID_ROL")]
        public long ID_ROL { get; set; }

        [Required]
        [Column("HABILITADO")]
        public bool HABILITADO { get; set; }

        public virtual USUARIO FK_ID_USUARIO { get; set; }
        public virtual DETALLE_MASTER FK_ID_ROL { get; set; }
    }
}
