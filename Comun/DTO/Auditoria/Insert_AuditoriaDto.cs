using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Auditoria
{
    public class Insert_AuditoriaDto
    {
        public  long ID { get; set;}
        public int Id_Tipo_Auditoria { get; set;}
        public string Ip_Maquina { get; set;}
        public DateTime fecha { get; set;}
        public long Id_Usuario { get; set;}
        public string Observacion { get; set;}

        
    }
}
