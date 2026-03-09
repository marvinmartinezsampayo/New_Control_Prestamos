using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Prestamo
{
    public class PrestamoResumenDto
    {
        public long ID_SOLICITUD { get; set; }
        public long NUMERO_IDENTIFICACION { get; set; }
        public string P_NOMBRE_SOLICITANTE { get; set; }
        public string S_NOMBRE_SOLICITANTE { get; set; }
        public string P_APELLIDO_SOLICITANTE { get; set; }
        public string S_APELLIDO_SOLICITANTE { get; set; }
        public decimal POR_INTERES { get; set; }
        public string CATEGORIA { get; set; }
        public decimal MONTO { get; set; }
        public decimal MULTAS { get; set; }
        public decimal SALDO_MORA { get; set; }
        public decimal RETORNO { get; set; }
        public decimal RETORNO_MES { get; set; }
        public decimal INTERES { get; set; }
        public decimal INTERES_MES { get; set; }
        public decimal SALDO_PENDIENTE { get; set; }
        public decimal INTERES_A_COBRAR { get; set; }
        public long ID_ESTADO { get; set; }
        public string ESTADO_PRESTAMO { get; set; }
    }
}
