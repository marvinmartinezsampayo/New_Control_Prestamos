using Comun.DTO.CodigoAcceso;
using Comun.Generales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.CodigoAcceso
{
    public interface IListarCodigosAcceso
    {
        Task<RespuestaDto<TReturn>> ObtenerCodigosDeHoyGenerico<TReturn>();
        Task<RespuestaDto<TReturn>> ObtenerCodigosPorFechasAsync<TReturn>(DateTime fechaInicio, DateTime fechaFin);
        Task<RespuestaDto<string>> ActualizarCodigoAccesoAsync(UpdateCodigoosAcccesoDto dto);
    }
}
