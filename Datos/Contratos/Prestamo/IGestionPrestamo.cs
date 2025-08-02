using Comun.Generales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Contratos.Prestamo
{
    public interface IGestionPrestamo
    {
        Task<RespuestaDto<TReturn>> Obtener_X_Identificacion_Async<TParam, TReturn>(TParam _modelo);
    }
}
