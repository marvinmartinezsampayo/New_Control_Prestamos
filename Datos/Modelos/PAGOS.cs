using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Modelos
{
    [Table("PAGOS")]
    public class PAGOS
    {
        [Key]
        [Column("ID")]
        public long ID { get; set; }

        [Required]
        [Column("ID_PRESTAMO")]
        public long ID_PRESTAMO { get; set; }

        [Required]
        [Column("FECHA_PAGO")]
        public DateTime FECHA_PAGO { get; set; }

        [Required]
        [Column("MONTO")]
        public long MONTO { get; set; }

        [Required]
        [Column("NUMERO_CUOTA")]
        [Range(1, 999)]  // El número de cuota debe estar entre 1 y 999
        public int NUMERO_CUOTA { get; set; }

        [Column("ID_TIPO_PAGO")]
        public long? ID_TIPO_PAGO { get; set; }

        // Relaciones
        [ForeignKey(nameof(ID_PRESTAMO))]
        public virtual PRESTAMOS FK_ID_PRESTAMO { get; set; }

        [ForeignKey(nameof(ID_TIPO_PAGO))]
        public virtual DETALLE_MASTER FK_ID_TIPO_PAGO { get; set; }
    }
}
