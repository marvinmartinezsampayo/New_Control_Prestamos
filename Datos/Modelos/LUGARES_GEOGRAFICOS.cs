using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Modelos
{
    [Table("LUGARES_GEOGRAFICOS")]
    public class LUGARES_GEOGRAFICOS
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // AutoIncrement
        [Column("ID")]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("DESCRIPCION")]
        public string Descripcion { get; set; }

        [Required]
        [StringLength(10)]
        [Column("TIPO_LUGAR")]
        public string TipoLugar { get; set; }

        [Required]
        [Column("CODIGO_DANE")]
        public long CodigoDane { get; set; }

        [Column("ID_DANE_PADRE")]
        public long? IdDanePadre { get; set; }

        [Required]
        [Column("HABILITADO")]
        public bool Habilitado { get; set; }
    }
}
