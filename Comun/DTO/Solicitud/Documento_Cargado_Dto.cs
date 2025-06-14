using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Comun.DTO.Solicitud
{
    public class Documento_Cargado_Dto
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Tamanio { get; set; }
        public string Tipo { get; set; }
        public string Data { get; set; }        
        public bool Habilitado { get; set; }
    }
}

