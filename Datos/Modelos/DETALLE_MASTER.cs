using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Modelos
{   
    [Table("DETALLE_MASTER")]
    public class DETALLE_MASTER
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("NOMBRE")]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("ABREVIACION")]
        public string Abreviacion { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("DESCRIPCION")]
        public string Descripcion { get; set; }

        [Required]
        [ForeignKey("FK_ID_MASTER_PAPA")]
        [Column("ID_TIPO")]
        public long? ID_TIPO { get; set; }  

        [Column("HABILITADO")]
        public bool Habilitado { get; set; }

        public virtual DETALLE_MASTER FK_ID_MASTER_PAPA { get; set; }
    }
}
