using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Prestamo
{
    public class Parametros_Insertar_Multas_Dto
    {
        public long IdPrestamo { get; set; }
        public long ValorMulta { get; set; }
        public long SaldoMulta { get; set; }
        public DateTime FechaImposicion { get; set; } = DateTime.Now;
        public long IdMotivo { get; set; }
        public string DescripcionMotivo { get; set; } = string.Empty;
        public long IdEstado { get; set; }
        public string UsuarioCreacion { get; set; } = string.Empty;
        public string MaquinaCreacion { get; set; } = string.Empty;
    }
}
