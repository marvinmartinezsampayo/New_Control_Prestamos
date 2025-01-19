using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Modelos
{
    [Table("BARRIOS")]
    public class BARRIOS
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // AutoIncrement
        [Column("ID")]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("NOMBRE")]
        public string Nombre { get; set; }

       
        [Required]
        [Column("CODIGO_DANE_MPIO")]
        public long CodigoDaneMpio { get; set; }

        [Required]
        [StringLength(100)]
        [Column("NOMBRE_MPIO")]
        public string NombreMpio { get; set; }

        [Required]
        [Column("HABILITADO")]
        public bool Habilitado { get; set; }

    }
}
