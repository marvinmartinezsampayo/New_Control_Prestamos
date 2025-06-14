using Comun.DTO.Solicitud;
using Comun.Enumeracion;
using Comun.Generales;
using Comun.Seguridad;
using Datos.Contexto;
using Datos.Contratos.Solicitud;
using Datos.Modelos;
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
                               .Where(x => x.Codigo_Acceso == param.ToString())
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

        public Task<RespuestaDto<TReturn>> ObtenerAsync<TReturn>()
        {
            throw new NotImplementedException();
        }

        public Task<RespuestaDto<TReturn>> ObtenerAsync<TParam, TReturn>(TParam _modelo)
        {
            throw new NotImplementedException();
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
                        GeneroId = parametros.IdGenero,
                        Email = parametros.Email,
                        Celular = parametros.Celular,
                        Codigo_Acceso = parametros.CodigoAcceso,
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
               
    }
}
