using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Modelos
{
    [Table("PRESTAMOS")]
    public class PRESTAMOS
    {
        [Key]
        [Column("ID")]
        public long ID { get; set; }

        [Required]
        [Column("ID_SOLICITUD")]
        public long ID_SOLICITUD { get; set; }

        [Required]
        [Column("MONTO")]
        public long MONTO { get; set; }

        [Required]
        [Column("NUMERO_CUOTAS")]
        [Range(1, 999)]  // El número de cuotas debe estar entre 1 y 999
        public int NUMERO_CUOTAS { get; set; }

        [Column("ID_PERIODICIDAD")]
        public long ID_PERIODICIDAD { get; set; }

        [Required]
        [Column("INTERES")]
        [Range(1, 999)]  // El interés debe estar entre 1 y 999
        public int INTERES { get; set; }

        [Required]
        [Column("FECHA_INICIO")]
        public DateTime FECHA_INICIO { get; set; }

        [Required]
        [Column("FECHA_FIN")]
        public DateTime FECHA_FIN { get; set; }

        [Column("SALDO_MONTO")]
        public long SALDO_MONTO { get; set; }

        [Column("ID_ESTADO")]
        public long ID_ESTADO { get; set; }

        // Relaciones
        [ForeignKey(nameof(ID_SOLICITUD))]
        public virtual SOLICITUD_PRESTAMO FK_ID_SOLICITUD { get; set; }

        [ForeignKey(nameof(ID_PERIODICIDAD))]
        public virtual DETALLE_MASTER FK_ID_PERIODICIDAD { get; set; }

        [ForeignKey(nameof(ID_ESTADO))]
        public virtual DETALLE_MASTER FK_ID_ESTADO { get; set; }

    }
}
