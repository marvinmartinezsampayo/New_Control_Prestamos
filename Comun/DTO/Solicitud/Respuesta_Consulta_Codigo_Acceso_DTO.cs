using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Solicitud
{
    public class Respuesta_Consulta_Codigo_Acceso_DTO
    {
        
        public long Id { get; set; }        
        public string Codigo { get; set; }        
        public DateTime FechaInicio { get; set; }        
        public DateTime FechaFin { get; set; }
        public string EmailAsociado { get; set; }        
        public string CelularAsociado { get; set; }       
        public long CantidadRegistros { get; set; }        
        public string Imagen { get; set; }
        public bool Habilitado { get; set; }       
        public string UsuarioCreacion { get; set; }        
        public string MaquinaCreacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}
