using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.DTO.Generales
{
    public class UsuarioDto
    {
        public long ID { get; set; }
        public string USUARIO_EMPRESARIAL { get; set; }
        public long NRO_IDENTIFICACION { get; set; }
        public long ID_TIPO_IDENTIFICACION { get; set; }
        public string PRIMER_NOMBRE { get; set; }
        public string SEGUNDO_NOMBRE { get; set; }
        public string PRIMER_APELLIDO { get; set; }
        public string SEGUNDO_APELLIDO { get; set; }
        public string EMAIL { get; set; }
        public string TELEFONO { get; set; }
        public string CONTRASENA { get; set; }
        public bool HABILITADO { get; set; }
        public Byte[]? FOTO { get; set; }
    }
}
