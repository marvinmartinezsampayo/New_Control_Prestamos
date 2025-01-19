using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Modelos
{
    [Table("DOCUMENTOS_X_SOLICITUD")]
    public class DOCUMENTOS_X_SOLICITUD
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long Id { get; set; }

        [Required]
        [ForeignKey("FK_DOCUMENTO")]
        [Column("ID_DOCUMENTO")]
        public long DocumentoId { get; set; }
        public virtual DOCUMENTOS FK_DOCUMENTO { get; set; }

        [Required]
        [Column("HABILITADO")]
        public bool Habilitado { get; set; }
    }
}
