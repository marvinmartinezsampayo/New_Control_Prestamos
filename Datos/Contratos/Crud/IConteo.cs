using Comun.Generales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Contratos.Crud
{
    public interface IConteo
    {
        Task<RespuestaDto<TReturn>> ContarAsync<TParam, TReturn>(TParam param);
    }
}
