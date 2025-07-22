using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Modelos
{
    [Table("auditoria")]
    public class AUDITORIA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long Id { get; set; }

        [Required]
        [Column("ID_TIPO_AUDITORIA")]
        public long IdTipoAuditoria { get; set; }

        [Required]
        [StringLength(100)]
        [Column("IP_MAQUINA")]
        public string IpMaquina { get; set; }

        [Required]
        [Column("FECHA")]
        public DateTime Fecha { get; set; }

        [Required]
        [Column("ID_USUARIO_AUDITADO")]
        public long IdUsuarioAuditado { get; set; }

        [Column("OBSERVACION")]
        [StringLength(4000)]
        public string? Observacion { get; set; }
    }
}
