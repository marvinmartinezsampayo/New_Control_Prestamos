using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Modelos
{
    [Serializable]
    [Table(nameof(DOCUMENTOS_REQUERIDOS))]
    public class DOCUMENTOS_REQUERIDOS
    {
        [Key]
        [Column("ID")]
        public long ID { get; set; }

        
        [Required]
        [Column("ID_DOCUMENTO")]
        [ForeignKey("FK_DOCUMENTOS")]
        public long ID_DOCUMENTO { get; set; }

        // Propiedad de navegación
        public virtual DOCUMENTOS FK_DOCUMENTOS { get; set; } = null!;

    }
}
