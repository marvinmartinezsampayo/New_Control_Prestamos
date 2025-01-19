using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Solicitud
{
    public class Respuesta_Consulta_Lugares_Geograficos_DTO
    {
        public long Id { get; set; }        
        public string Descripcion { get; set; }        
        public string TipoLugar { get; set; }       
        public long CodigoDane { get; set; }       
        public long? IdDanePadre { get; set; }        
        public bool Habilitado { get; set; }
    }
}
