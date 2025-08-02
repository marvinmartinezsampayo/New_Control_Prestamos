using Comun.Generales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Contratos.Auditoria
{
    public interface IInsert_Auditoria
    {
        Task<RespuestaDto<long>> InsertarAuditoriaAsync<TParam>(TParam _modelo);
    }
}
