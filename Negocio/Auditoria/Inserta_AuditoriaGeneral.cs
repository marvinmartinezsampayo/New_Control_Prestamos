using Comun.DTO.Auditoria;
using Comun.DTO.Generales;
using Comun.Enumeracion;
using Comun.Generales;
using Datos.Contexto;
using Datos.Contratos.Auditoria;
using Datos.Contratos.Solicitud;
using Datos.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Gestion
{
    public class Inserta_AuditoriaGeneral : IInsert_Auditoria
    {
        private readonly IConfiguration _configuration;
        private readonly ContextoGeneral _context;

        public Inserta_AuditoriaGeneral(IConfiguration configuration, ContextoGeneral context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<RespuestaDto<long>> InsertarAuditoriaAsync<TParam>(TParam _modelo)
        {
            if (_modelo is not Insert_AuditoriaDto param)
            {
                return new RespuestaDto<long>
                {
                    Codigo = EstadoOperacion.Malo,
                    Mensaje = "Parámetros inválidos",
                    Respuesta = 0
                };
            }

            try
            {
                var auditoria = new AUDITORIA
                {
                    IdTipoAuditoria = param.Id_Tipo_Auditoria,
                    IpMaquina = param.Ip_Maquina,
                    Fecha = param.fecha, // o DateTime.Now si quieres forzar la fecha actual
                    IdUsuarioAuditado = param.Id_Usuario,
                    Observacion = param.Observacion
                };

                _context.AUDITORIA.Add(auditoria);
                await _context.SaveChangesAsync();

                return new RespuestaDto<long>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "Auditoría registrada correctamente.",
                    Respuesta = auditoria.Id
                };
            }
            catch (Exception ex)
            {
                return new RespuestaDto<long>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "Error al registrar auditoría.",
                    Respuesta = 0
                };
            }
        }




    }
}
