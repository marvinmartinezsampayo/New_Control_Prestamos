using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Prestamo
{
    public class Prestamo_Dto
    {
        public long ID { get; set; }
        public long ID_SOLICITUD { get; set; }
        public string SOLICITANTE { get; set; }
        public long MONTO { get; set; }
        public long NUMERO_CUOTAS { get; set; }
        public long ID_PERIODICIDAD { get; set; }
        public string PERIODICIDAD { get; set; }
        public long INTERES { get; set; }
        public DateTime FECHA_INICIO { get; set; }
        public DateTime FECHA_FIN { get; set; }
        public long SALDO_MONTO { get; set; }
        public long ID_ESTADO { get; set; }
        public string ESTADO { get; set; }
    }
}
