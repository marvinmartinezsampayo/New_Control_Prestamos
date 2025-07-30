using Comun.Generales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.CodigoAcceso
{
    public interface IInsertCodigoAcceso
    {
        Task<RespuestaDto<long>> InsertarCodigoAccesoAsync<TParam>(TParam _modelo);
    }
}
