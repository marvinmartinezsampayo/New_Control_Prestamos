using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Prestamo
{
    public class Parametros_Actualizar_Multas_Dto
    {
        public long Id { get; set; }
        public long IdPrestamo { get; set; }
        public long ValorMulta { get; set; }
        public long SaldoMulta { get; set; }
        public DateTime FechaImposicion { get; set; }
        public long IdMotivo { get; set; }
        public string DescripcionMotivo { get; set; } = string.Empty;
        public long IdEstado { get; set; }
        public string UsuarioActualizacion { get; set; } = string.Empty;
        public string MaquinaActualizacion { get; set; } = string.Empty;
    }
}
