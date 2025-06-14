using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Solicitud
{
    public class Respuesta_Consulta_Barrios_Dto
    {  
        public long Id { get; set; }
        public string Nombre { get; set; }       
        public long CodigoDaneMpio { get; set; }
        public string NombreMpio { get; set; }
        public bool Habilitado { get; set; }
    }
}
