using Comun.Generales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Contratos.Crud
{
    public interface IDeshabilitar
    {
        Task<RespuestaDto<TReturn>> DeshabilitarAsync<TParam, TReturn>(TParam _identificador);
    }
}
