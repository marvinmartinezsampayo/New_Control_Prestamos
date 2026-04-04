using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Datos.Modelos
{
    [Table("DEPOSITO_INVERSOR")]
    public class DEPOSITO_INVERSOR
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }

        [Required]
        [Column("ID_USUARIO")]
        public long IdUsuario { get; set; }

        [Required]
        [Column("FECHA_DEPOSITO")]
        public DateTime FechaDeposito { get; set; } = DateTime.Now;

        [Required]
        [Column("VALOR_DEPOSITADO")]
        public decimal ValorDepositado { get; set; }

        [Required]
        [Column("ID_ESTADO")]
        public long IdEstado { get; set; }

        [Column("FECHA_RETIRO")]
        public DateTime? FechaRetiro { get; set; }

        [Column("VALOR_RETIRADO")]
        public decimal? ValorRetirado { get; set; }

        // Relaciones
        [ForeignKey(nameof(IdUsuario))]
        public virtual USUARIO FK_ID_USUARIO { get; set; }

        [ForeignKey(nameof(IdEstado))]
        public virtual DETALLE_MASTER FK_ID_ESTADO { get; set; }
    }
}
