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

        //Tablero de control
        Task<RespuestaDto<TReturn>> Obtener_All_Prestamos_Async<TParam, TReturn>(TParam _modelo);
        Task<RespuestaDto<TReturn>> Obtener_Sumatoria_Depositos_Inversores_Async<TReturn>();
        Task<RespuestaDto<TReturn>> Obtener_Sumatoria_Saldos_Creditos_Async<TReturn>();
        Task<RespuestaDto<TReturn>> Obtener_Sumatoria_Monto_Pagos_Async<TParam, TReturn>(TParam _modelo);
        Task<RespuestaDto<TReturn>> Insertar_Multa_Async<TParam, TReturn>(TParam _modelo);
        Task<RespuestaDto<TReturn>> Actualizar_Multa_Async<TParam, TReturn>(TParam _modelo);
        Task<RespuestaDto<TReturn>> Pago_Multa_Async<TParam, TReturn>(TParam _modelo);
    }
}
