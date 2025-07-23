using Comun.Generales;
using Datos.Contratos.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Contratos.Solicitud
{
    public interface IRegistroSolicitud:IObtener, IConteo, IGuardar,IHabilitar,IDeshabilitar
    {
        Task<RespuestaDto<TReturn>> Obtener_X_Id_Async<TParam, TReturn>(TParam _modelo);
        Task<RespuestaDto<bool>> ActualizarEstadoSolicitudAsync<TParam>(TParam _modelo);
    }
}
