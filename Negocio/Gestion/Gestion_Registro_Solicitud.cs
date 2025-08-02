using Comun.DTO.Solicitud;
using Comun.Enumeracion;
using Comun.Generales;
using Comun.Seguridad;
using Datos.Contexto;
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
    public class Gestion_Registro_Solicitud : IRegistroSolicitud
    {
        private readonly IConfiguration _configuration;
        private readonly IAlmacenarDocumentos _documentos;
        private readonly ContextoGeneral _context;
       

        public Gestion_Registro_Solicitud(IConfiguration configuration, ContextoGeneral context, IAlmacenarDocumentos documentos)
        {
            _configuration = configuration;
            _context = context;
            _documentos = documentos;
        }

        public async Task<RespuestaDto<TReturn>> ContarAsync<TParam, TReturn>(TParam param)
        {
            try
            {
                long cantidad = _context.SOLICITUD_PRESTAMO
                               .Where(x => x.CodigoAcceso == param.ToString())
                               .Count();

                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "Operación realizada correctamente.",
                    Respuesta = (TReturn)Convert.ChangeType(cantidad, typeof(TReturn))
                };
            }
            catch (Exception ex)
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "Se generó una excepción al ejecutar la acción.",
                    Respuesta = (TReturn)Convert.ChangeType("Ex3pt" + PasswordService.GenerarCodigo(5), typeof(TReturn))
                };
            }
        }

        public Task<RespuestaDto<TReturn>> DeshabilitarAsync<TParam, TReturn>(TParam _identificador)
        {
            throw new NotImplementedException();
        }

        public async Task<RespuestaDto<TReturn>> GuardarAsync<TParam, TReturn>(TParam _modelo)
        {
            try
            {
                if (_modelo is Parametros_Insert_Solicitud_Prestamo_Dto parametros)
                {

                    var rInsertSol = await Insert_Solicitud_Prestamo_Async(parametros);

                    if (rInsertSol.Estado)
                    {
                        long idSolicitud = rInsertSol.Respuesta;

                        //Aca guardamos los datos del documento

                        if (parametros.Documentos.Count > 0 && idSolicitud > 0)
                        {
                            foreach (var documento in parametros.Documentos)
                            {
                                documento.IdSolicitud = idSolicitud;
                                var doc = await _documentos.GuardarAsync<Parametros_Add_Documento_X_Solicitud_Dto, bool>(documento);
                            }

                            return new RespuestaDto<TReturn>
                            {
                                Codigo = EstadoOperacion.Bueno,
                                Mensaje = "OK",
                                Respuesta = (TReturn)Convert.ChangeType(true, typeof(TReturn))
                            };
                        }
                        else
                        {
                            return new RespuestaDto<TReturn>
                            {
                                Codigo = EstadoOperacion.Malo,
                                Mensaje = "FALLO",
                                Respuesta= (TReturn)Convert.ChangeType(false, typeof(TReturn))
                            };
                        }

                    }
                    else
                    {
                        return new RespuestaDto<TReturn>
                        {
                            Codigo = EstadoOperacion.Malo,
                            Mensaje = "ERROR",
                            Respuesta = (TReturn)Convert.ChangeType(false, typeof(TReturn))
                        };
                    }
                }
                else 
                {
                    return new RespuestaDto<TReturn>
                    {
                        Codigo = EstadoOperacion.Malo,
                        Mensaje = "ERROR",
                        Respuesta = (TReturn)Convert.ChangeType(false, typeof(TReturn))
                    };
                }
                
            }
            catch (Exception ex)
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "Excepcion al Insertar Solicitud",
                    Respuesta = (TReturn)Convert.ChangeType(false, typeof(TReturn))
                };
            }
        }

        public Task<RespuestaDto<TReturn>> HabilitarAsync<TParam, TReturn>(TParam _identificador)
        {
            throw new NotImplementedException();
        }

        public async Task<RespuestaDto<TReturn>> ObtenerAsync<TReturn>()
        {
            List<Respuesta_Consulta_Solicitudes_Prestamo_Dto> resp = new List<Respuesta_Consulta_Solicitudes_Prestamo_Dto>();
                        
                var consulta = await _context.SOLICITUD_PRESTAMO
                                    .Where(s => s.Habilitado)
                                    .OrderByDescending(s => s.FechaCreacion)
                                    .Select(s => new Respuesta_Consulta_Solicitudes_Prestamo_Dto
                                    {
                                        Id = s.Id,
                                        PrimerNombreSolicitante = s.PrimerNombreSolicitante,
                                        SegundoNombreSolicitante = s.SegundoNombreSolicitante,
                                        PrimerApellidoSolicitante = s.PrimerApellidoSolicitante,
                                        SegundoApellidoSolicitante = s.SegundoApellidoSolicitante,
                                        TipoIdentificacionId = s.TipoIdentificacionId,
                                        NumeroIdentificacion = s.NumeroIdentificacion,
                                        DepartamentoResidenciaId = s.DepartamentoResidenciaId,
                                        MunicipioResidenciaId = s.MunicipioResidenciaId,
                                        BarrioResidenciaId = s.BarrioResidenciaId,
                                        DireccionResidencia = s.DireccionResidencia,
                                        EstadoId = s.EstadoId,
                                        Monto = s.Monto,
                                        Email = s.Email,
                                        Celular = s.Celular,
                                        CodigoAcceso = s.CodigoAcceso,
                                        Habilitado = s.Habilitado,
                                        FechaCreacion = s.FechaCreacion
                                    })
                                    .ToListAsync();

                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "OK",
                    Respuesta = (TReturn)Convert.ChangeType(consulta, typeof(TReturn))
                };
            
        }

        public async Task<RespuestaDto<TReturn>> ObtenerAsync<TParam, TReturn>(TParam _modelo)
        {
            List<Respuesta_Consulta_Solicitudes_Prestamo_Dto> resp = new List<Respuesta_Consulta_Solicitudes_Prestamo_Dto>();

            if (_modelo is Parametros_Consulta_Solicitudes_X_Estado_Dto parametros)
            {
                var consulta =  await _context.SOLICITUD_PRESTAMO
                                    .Where(s => s.EstadoId == parametros.ID_ESTADO && s.Habilitado)
                                    .OrderByDescending(s => s.FechaCreacion)
                                    .Select(s => new Respuesta_Consulta_Solicitudes_Prestamo_Dto
                                    {
                                        Id = s.Id,
                                        PrimerNombreSolicitante = s.PrimerNombreSolicitante,
                                        SegundoNombreSolicitante = s.SegundoNombreSolicitante,
                                        PrimerApellidoSolicitante = s.PrimerApellidoSolicitante,
                                        SegundoApellidoSolicitante = s.SegundoApellidoSolicitante,
                                        TipoIdentificacionId = s.TipoIdentificacionId,
                                        NumeroIdentificacion = s.NumeroIdentificacion,
                                        DepartamentoResidenciaId = s.DepartamentoResidenciaId,
                                        MunicipioResidenciaId = s.MunicipioResidenciaId,
                                        BarrioResidenciaId = s.BarrioResidenciaId,
                                        DireccionResidencia = s.DireccionResidencia,
                                        EstadoId = s.EstadoId,
                                        Monto = s.Monto,
                                        Email = s.Email,
                                        Celular = s.Celular,
                                        CodigoAcceso = s.CodigoAcceso,
                                        Habilitado = s.Habilitado,
                                        FechaCreacion = s.FechaCreacion
                                    })
                                    .ToListAsync();

                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "OK",
                    Respuesta = (TReturn)Convert.ChangeType(consulta, typeof(TReturn))
                };
            }
            else
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Malo,
                    Mensaje = "ERROR",
                    Respuesta = (TReturn)Convert.ChangeType(resp, typeof(TReturn))
                };
            }         

        }

        public async Task<RespuestaDto<TReturn>> Obtener_X_Id_Async<TParam, TReturn>(TParam _modelo)
        {
            Respuesta_Consulta_Solicitudes_Prestamo_Dto resp = new Respuesta_Consulta_Solicitudes_Prestamo_Dto();

            if (_modelo is Int64 IdSolicitud)
            {
                var consulta = await _context.SOLICITUD_PRESTAMO
                                    .Where(s => s.Id == IdSolicitud && s.Habilitado)
                                    .OrderByDescending(s => s.FechaCreacion)
                                    .Select(s => new Respuesta_Consulta_Solicitudes_Prestamo_Dto
                                    {
                                        Id = s.Id,
                                        PrimerNombreSolicitante = s.PrimerNombreSolicitante,
                                        SegundoNombreSolicitante = s.SegundoNombreSolicitante,
                                        PrimerApellidoSolicitante = s.PrimerApellidoSolicitante,
                                        SegundoApellidoSolicitante = s.SegundoApellidoSolicitante,
                                        TipoIdentificacionId = s.TipoIdentificacionId,
                                        NumeroIdentificacion = s.NumeroIdentificacion,
                                        DepartamentoResidenciaId = s.DepartamentoResidenciaId,
                                        MunicipioResidenciaId = s.MunicipioResidenciaId,
                                        BarrioResidenciaId = s.BarrioResidenciaId,
                                        DireccionResidencia = s.DireccionResidencia,
                                        EstadoId = s.EstadoId,
                                        Monto = s.Monto,
                                        Email = s.Email,
                                        Celular = s.Celular,
                                        CodigoAcceso = s.CodigoAcceso,
                                        Habilitado = s.Habilitado,
                                        FechaCreacion = s.FechaCreacion
                                    })
                                    .FirstOrDefaultAsync();

                consulta.Documentos = await _context.DOCUMENTOS_X_SOLICITUD
                                        .Where(x=>x.IdSolicitud == IdSolicitud)
                                         .Select(s => new Parametros_Add_Documento_X_Solicitud_Dto
                                         { 
                                             IdSolicitud = s.IdSolicitud,
                                             IdDocumento = s.IdDocumento,
                                             ContenidoDoc = s.ContenidoDoc,
                                             Formato = s.Formato,
                                             Tamanio = s.Tamanio,
                                             UsuarioCreacion = s.UsuarioCreacion,
                                             MaquinaCreacion = s.MaquinaCreacion,
                                             Habilitado = s.Habilitado                                         
                                         })
                                        .ToListAsync();


                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "OK",
                    Respuesta = (TReturn)Convert.ChangeType(consulta, typeof(TReturn))
                };
            }
            else
            {
                return new RespuestaDto<TReturn>
                {
                    Codigo = EstadoOperacion.Malo,
                    Mensaje = "ERROR",
                    Respuesta = (TReturn)Convert.ChangeType(resp, typeof(TReturn))
                };
            }
        }        

        private async Task<RespuestaDto<long>> Insert_Solicitud_Prestamo_Async (Parametros_Insert_Solicitud_Prestamo_Dto _modelo)
        {
            try
            {
                if (_modelo is Parametros_Insert_Solicitud_Prestamo_Dto parametros)
                {

                    var nuevaSolicitud = new SOLICITUD_PRESTAMO
                    {
                        Id = 0,
                        PrimerNombreSolicitante = parametros.PNombreSolicitante,
                        SegundoNombreSolicitante = parametros.SNombreSolicitante,
                        PrimerApellidoSolicitante = parametros.PApellidoSolicitante,
                        SegundoApellidoSolicitante = parametros.SApellidoSolicitante,
                        TipoIdentificacionId = parametros.TipoIdentificacion,
                        NumeroIdentificacion = parametros.NumeroIdentificacion,
                        DepartamentoResidenciaId = parametros.IdDeptoResidencia,
                        MunicipioResidenciaId = parametros.IdMpioResidencia,
                        BarrioResidenciaId = parametros.IdBarrioResidencia,
                        DireccionResidencia = parametros.DireccionResidencia,
                        EstadoId = parametros.EstadoId,
                        Monto = parametros.Monto,
                        Email = parametros.Email,
                        Celular = parametros.Celular,
                        CodigoAcceso = parametros.CodigoAcceso,
                        Habilitado = true, 
                        UsuarioCreacion = parametros.UsuarioCreacion ?? "SISTEMA",
                        MaquinaCreacion = Environment.MachineName,
                        FechaCreacion = DateTime.Now
                    };

                    _context.SOLICITUD_PRESTAMO.Add(nuevaSolicitud);
                    await _context.SaveChangesAsync();
                    long idGenerado =  nuevaSolicitud.Id;
                                        
                    return new RespuestaDto<long>
                    {
                        Codigo = EstadoOperacion.Bueno,
                        Mensaje = "OK",
                        Respuesta = idGenerado
                    };
                }
                else
                {
                    return new RespuestaDto<long>
                    {
                        Codigo = EstadoOperacion.Malo,
                        Mensaje = "Datos insuficientes para insertar solicitud",
                        Respuesta = 0
                    };
                }

            }
            catch (Exception ex)
            {
                return new RespuestaDto<long>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje="Error al Insertar Solicitud",
                    Respuesta=0
                };
            }
        }

        //metodo para actualziar el estado 
        public async Task<RespuestaDto<bool>> ActualizarEstadoSolicitudAsync<TParam>(TParam _modelo)
        {
            if (_modelo is not Parametros_Actualizar_Estado_Solicitud_Dto param)
            {
                return new RespuestaDto<bool>
                {
                    Codigo = EstadoOperacion.Malo,
                    Mensaje = "No hay datos suficientes para ejecutar la acción.",
                    Respuesta = false
                };
            }

            try
            {
                var solicitud = await _context.SOLICITUD_PRESTAMO.FirstOrDefaultAsync(x => x.Id == param.IdSolicitud);

                if (solicitud == null)
                {
                    return new RespuestaDto<bool>
                    {
                        Codigo = EstadoOperacion.Malo,
                        Mensaje = "No se encontró la solicitud especificada.",
                        Respuesta = false
                    };
                }

                solicitud.EstadoId = param.NuevoEstadoId;
                await _context.SaveChangesAsync();

                return new RespuestaDto<bool>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "Estado actualizado correctamente.",
                    Respuesta = true
                };
            }
            catch (Exception ex)
            {
                return new RespuestaDto<bool>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "Ocurrió un error al actualizar el estado.",
                    Respuesta = false
                };
            }
        }

        
    }
}
