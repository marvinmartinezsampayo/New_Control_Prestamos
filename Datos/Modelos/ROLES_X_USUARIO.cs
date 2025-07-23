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
        [Column("ID_USUARIO")]
        public long ID_USUARIO { get; set; }

        [Required]
        [Column("ID_ROL")]
        public long ID_ROL { get; set; }

        [Required]
        [Column("HABILITADO")]
        public bool HABILITADO { get; set; }

        // Ambas propiedades deben apuntar a DETALLE_MASTER según tu esquema de BD
        [ForeignKey(nameof(ID_USUARIO))]
        public virtual DETALLE_MASTER FK_ID_USUARIO { get; set; }

        [ForeignKey(nameof(ID_ROL))]
        public virtual DETALLE_MASTER FK_ID_ROL { get; set; }
    }

}
