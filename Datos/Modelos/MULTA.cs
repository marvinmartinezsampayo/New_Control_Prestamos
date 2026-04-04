using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Modelos
{
    [Table("MULTA")]
    public class MULTA
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column("ID_PRESTAMO")]
        public long IdPrestamo { get; set; }

        [Required]
        [Column("VALOR_MULTA")]
        public long ValorMulta { get; set; }

        [Required]
        [Column("SALDO_MULTA")]
        public long SaldoMulta { get; set; }

        [Required]
        [Column("FECHA_IMPOSICION")]
        public DateTime FechaImposicion { get; set; } = DateTime.Now;

        [Required]
        [Column("ID_MOTIVO")]
        public long IdMotivo { get; set; }

        [Required]
        [Column("DESCRIPCION_MOTIVO")]
        [MaxLength(500)]
        public string DescripcionMotivo { get; set; } = string.Empty;

        [Required]
        [Column("ID_ESTADO")]
        public long IdEstado { get; set; }

        [Required]
        [Column("USUARIO_CREACION")]
        [MaxLength(100)]
        public string UsuarioCreacion { get; set; } = string.Empty;

        [Required]
        [Column("MAQUINA_CREACION")]
        [MaxLength(100)]
        public string MaquinaCreacion { get; set; } = string.Empty;

        [Required]
        [Column("FECHA_CREACION")]
        public DateTime FechaCreacion { get; set; }

        [Required]
        [Column("USUARIO_ACTUALIZACION")]
        [MaxLength(100)]
        public string UsuarioActualizacion { get; set; } = string.Empty;

        [Required]
        [Column("MAQUINA_ACTUALIZACION")]
        [MaxLength(100)]
        public string MaquinaActualizacion { get; set; } = string.Empty;

        [Required]
        [Column("FECHA_ACTUALIZACION")]
        public DateTime FechaActualizacion { get; set; }

        // Navigation Properties
        [ForeignKey("IdPrestamo")]
        public virtual PRESTAMOS? Prestamo { get; set; }

        [ForeignKey("IdEstado")]
        public virtual DETALLE_MASTER? Estado { get; set; }
    }
}
