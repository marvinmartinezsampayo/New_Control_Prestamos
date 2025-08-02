using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Prestamo
{
    public class RegistrarPagoDto
    {
        public int ID_PRESTAMO { get; set; }

        [Required(ErrorMessage = "La fecha de pago es requerida")]
        public DateTime FECHA_PAGO { get; set; }

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero")]
        public decimal MONTO { get; set; }
    }
}
