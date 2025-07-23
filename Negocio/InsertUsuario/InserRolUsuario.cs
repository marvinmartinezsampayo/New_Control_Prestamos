using Comun.DTO.InsertRolUsuario;
using Comun.Enumeracion;
using Comun.Generales;
using Comun.Mapeo;
using Datos.Administracion;
using Datos.Contexto;
using Datos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.InsertUsuario
{
    public class InserRolUsuario : IInsertRolUsuario
    {
        private readonly ContextoGeneral context;

        public InserRolUsuario(ContextoGeneral _context)
        {
            context = _context;
        }

        public async Task<RespuestaDto<bool>> InsertarRolUsuarioAsync(InsertRolUsuarioDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return new RespuestaDto<bool>
                    {
                        Codigo = EstadoOperacion.Malo,
                        Mensaje = "El objeto de rol por usuario es nulo.",
                        Respuesta = false
                    };
                }

                // Buscar si ya existe ese rol para ese usuario
                var registroExistente = await context.ROLES_X_USUARIO
                    .FirstOrDefaultAsync(r => r.ID_USUARIO == dto.Id_Usuario && r.ID_ROL == dto.Id_Rol);

                if (registroExistente != null)
                {
                    if (registroExistente.HABILITADO == dto.Habilitado)
                    {
                        return new RespuestaDto<bool>
                        {
                            Codigo = EstadoOperacion.Bueno,
                            Mensaje = $"El rol ya estaba {(dto.Habilitado ? "asignado" : "deshabilitado")}.",
                            Respuesta = true
                        };
                    }

                    // Si está deshabilitado y se quiere activar o viceversa
                    registroExistente.HABILITADO = dto.Habilitado;
                    context.ROLES_X_USUARIO.Update(registroExistente);
                }
               
                else
                {
                    // Crear nuevo registro manualmente sin mapeador
                    var entidad = new ROLES_X_USUARIO
                    {
                        ID_USUARIO = dto.Id_Usuario,
                        ID_ROL = dto.Id_Rol,
                        HABILITADO = dto.Habilitado
                       
                    };
                    await context.ROLES_X_USUARIO.AddAsync(entidad);
                }

                await context.SaveChangesAsync();

                return new RespuestaDto<bool>
                {
                    Codigo = EstadoOperacion.Bueno,
                    Mensaje = dto.Habilitado
                        ? "Rol asignado o reactivado correctamente."
                        : "Rol deshabilitado correctamente.",
                    Respuesta = true
                };
            }
            catch (Exception ex)
            {
                return new RespuestaDto<bool>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "Ocurrió un error al procesar el rol del usuario. Detalles: " + ex.Message,
                    Respuesta = false
                };
            }
        }


    }
}
