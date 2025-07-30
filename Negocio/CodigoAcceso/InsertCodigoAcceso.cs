using Comun.DTO.CodigoAcceso;
using Comun.Enumeracion;
using Comun.Generales;
using Datos.CodigoAcceso;
using Datos.Contexto;
using Datos.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.CodigoAcceso
{


    public class InsertCodigoAcceso : IInsertCodigoAcceso
    {

        private readonly IConfiguration _configuration;
        private readonly ContextoGeneral _context;

        public InsertCodigoAcceso(IConfiguration configuration, ContextoGeneral context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<RespuestaDto<long>> InsertarCodigoAccesoAsync<TParam>(TParam _modelo)
        {
            if (_modelo is not Codigo_AccesoDto param)
            {
                return new RespuestaDto<long>
                {
                    Codigo = EstadoOperacion.Malo,
                    Mensaje = "Parámetros inválidos",
                    Respuesta = 0
                };
            }

            // Validaciones manuales (por si acaso)
            if (string.IsNullOrWhiteSpace(param.Codigo) ||
                param.FechaInicio == default ||
                param.FechaFin == default ||
                string.IsNullOrWhiteSpace(param.UsuarioCreacion) ||
                string.IsNullOrWhiteSpace(param.MaquinaCreacion))
            {
                return new RespuestaDto<long>
                {
                    Codigo = EstadoOperacion.Malo,
                    Mensaje = "Faltan campos requeridos para el registro.",
                    Respuesta = 0
                };
            }

            try
            {
                var codigo = new CODIGO_ACCESO
                {
                    Codigo = param.Codigo,
                    FechaInicio = param.FechaInicio,
                    FechaFin = param.FechaFin,
                    EmailAsociado = param.EmailAsociado,
                    CelularAsociado = param.CelularAsociado,
                    CantidadRegistros = param.CantidadRegistros,
                    Imagen = param.Imagen,
                    Habilitado = param.Habilitado,
                    UsuarioCreacion = param.UsuarioCreacion,
                    MaquinaCreacion = param.MaquinaCreacion,
                    FechaCreacion = param.FechaCreacion ?? DateTime.Now,
                };


                _context.CODIGO_ACCESO.Add(codigo);
                await _context.SaveChangesAsync();

                return new RespuestaDto<long>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = "Código de acceso registrado correctamente.",
                    Respuesta = codigo.Id
                };
            }
            catch (DbUpdateException dbEx)
            {
                return new RespuestaDto<long>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "Error de base de datos al registrar el código. Detalle: " + dbEx.Message,
                    Respuesta = 0
                };
            }
            catch (Exception ex)
            {
                return new RespuestaDto<long>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "Ocurrió un error inesperado al registrar el código.",
                    Respuesta = 0
                };
            }
        }


    }
}
