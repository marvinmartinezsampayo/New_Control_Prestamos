using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Prestamo
{
    public class Parametros_Pago_Multas_Dto
    {
        public long Id { get; set; }
        public long SaldoMulta { get; set; }
        public long? IdEstado { get; set; }
        public string UsuarioActualizacion { get; set; } = string.Empty;
        public string MaquinaActualizacion { get; set; } = string.Empty;
    }
}
