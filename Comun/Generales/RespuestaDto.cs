using Comun.Enumeracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Generales
{
    public class RespuestaDto<T>
    {
        #region Propiedades
        public EstadoOperacion Codigo { get; set; }
        public string Mensaje { get; set; }
        public bool Estado { get => Codigo == EstadoOperacion.Bueno ? true : false; }
        public T Respuesta { get; set; }
        #endregion
    }
}
