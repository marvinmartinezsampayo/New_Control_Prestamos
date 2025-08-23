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

        Task<RespuestaDto<TReturn>> Obtener_X_ID_Async<TParam, TReturn>(TParam _modelo);

        Task<RespuestaDto<TReturn>> Insertar_Pago_Async<TParam, TReturn>(TParam _modelo);

        Task<RespuestaDto<TReturn>> Actualizar_Prestamo_Async<TParam, TReturn>(TParam _modelo);

        Task<RespuestaDto<TReturn>> Obtener_Pagos_Async<TParam, TReturn>(TParam _modelo);
    }
}
