using Comun.Generales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Contratos.Crud
{
    public interface IHabilitar
    {
        Task<RespuestaDto<TReturn>> HabilitarAsync<TParam, TReturn>(TParam _identificador);
    }
}
