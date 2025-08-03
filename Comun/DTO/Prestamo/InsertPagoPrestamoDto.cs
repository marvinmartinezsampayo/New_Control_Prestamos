using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Prestamo
{
    public class InsertPagoPrestamoDto
    {
        [Required(ErrorMessage = "El ID del préstamo es requerido")]
        public long ID_PRESTAMO { get; set; }

        [Required(ErrorMessage = "La fecha de pago es requerida")]
        [Display(Name = "Fecha de Pago")]
        [DataType(DataType.DateTime)]
        public DateTime FECHA_PAGO { get; set; }

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(1, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal MONTO { get; set; }

        [Display(Name = "Pagar solo intereses")]
        public bool PAGO_INTERESES { get; set; } = false;
    }
}
