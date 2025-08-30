using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Prestamo
{
    public class Pago_Dto
    {        
        public long ID { get; set; }        
        public long ID_PRESTAMO { get; set; }        
        public DateTime FECHA_PAGO { get; set; }        
        public decimal MONTO { get; set; }        
        public int? NUMERO_CUOTA { get; set; }        
        public long? ID_TIPO_PAGO { get; set; }
        public String TIPO_PAGO { get; set; }
    }
}
