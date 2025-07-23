using Comun.Generales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Contratos.Usuario
{
    public interface IBusacrUsuarioNurIdentificacion
    {
        Task<RespuestaDto<TReturn>> ObtenerUsuarioPorIdentificacionAsync<TParam, TReturn>(TParam _modelo);
    }
}
