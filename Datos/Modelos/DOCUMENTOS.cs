using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Modelos
{
    [Table("DOCUMENTOS")]
    public class DOCUMENTOS
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("NOMBRE")]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        [Column("DESCRIPCION")]
        public string Descripcion { get; set; }

        [Required]
        [Column("PESO_MAXIMO")]
        public long PesoMaximo { get; set; }

        [Required]
        [Column("HABILITADO")]
        public bool Habilitado { get; set; }
    }
}
