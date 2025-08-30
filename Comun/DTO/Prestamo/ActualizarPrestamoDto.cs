using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Prestamo
{
    public class ActualizarPrestamoDto
    {
        [Required(ErrorMessage = "El ID del prestamo es requerido")]
        public long ID { get; set; }

        [Required(ErrorMessage = "El saldo es requerido")]        
        public decimal SALDO { get; set; }
               
        public decimal? ID_ESTADO { get; set; }
    }
}
