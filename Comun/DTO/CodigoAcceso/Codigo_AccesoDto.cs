using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.CodigoAcceso
{
    public class Codigo_AccesoDto
    {
        public string Codigo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string EmailAsociado { get; set; }
        public string CelularAsociado { get; set; }
        public int CantidadRegistros { get; set; }
        public string Imagen { get; set; }
        public bool Habilitado { get; set; }
        public string UsuarioCreacion { get; set; }
        public string MaquinaCreacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public Int64 Id_Usuario { get; set; }
        public string CodigoAcesso { get; set; }
     
    }
}
