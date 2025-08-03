using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Prestamo
{
    public class RegistrarActualizarPagoDto
    {
        [Required(ErrorMessage = "El ID del pago es requerido")]
        public long ID { get; set; }

        [Required(ErrorMessage = "El ID del préstamo es requerido")]
        public long ID_PRESTAMO { get; set; }

        [Required(ErrorMessage = "La fecha de pago es requerida")]
        public DateTime FECHA_PAGO { get; set; }

        [Required(ErrorMessage = "El monto es requerido")]
        [Range(1, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal MONTO { get; set; }

        [Required(ErrorMessage = "El número de cuota es requerido")]
        [Range(1, long.MaxValue, ErrorMessage = "El número de cuota debe ser mayor a 0")]
        public long? NUMERO_CUOTA { get; set; }

        public long ID_TIPO_PAGO { get; set; }
        
    }
}
