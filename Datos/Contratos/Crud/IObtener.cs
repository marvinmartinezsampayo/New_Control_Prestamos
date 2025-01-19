using Comun.Generales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Contratos.Crud
{
    public interface IObtener
    {
        Task<RespuestaDto<TReturn>> ObtenerAsync<TReturn>();
        Task<RespuestaDto<TReturn>> ObtenerAsync<TParam, TReturn>(TParam _modelo);
    }
}
