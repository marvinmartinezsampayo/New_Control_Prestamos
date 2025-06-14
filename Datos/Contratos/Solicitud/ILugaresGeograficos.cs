using Comun.Generales;
using Datos.Contratos.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Contratos.Solicitud
{
    public interface ILugaresGeograficos:IObtener
    {
        Task<RespuestaDto<TReturn>> ObtenerBarriosAsync<TParam, TReturn>(TParam _modelo);
    }
}
