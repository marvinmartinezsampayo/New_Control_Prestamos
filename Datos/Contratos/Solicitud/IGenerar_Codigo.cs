using Comun.Generales;
using Datos.Contratos.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Contratos.Solicitud
{
    public interface IGenerar_Codigo:IObtener
    {
        Task<RespuestaDto<TReturn>> ObtenerListaDocAsync<TReturn>();
    }
}
