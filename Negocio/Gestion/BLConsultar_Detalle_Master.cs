using Comun.DTO.Generales;
using Datos.Contexto;
using Datos.Contratos.Solicitud;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Gestion
{
    public class BLConsultar_Detalle_Master : IBLConsultar_Detalle_Master
    {
        private readonly IConfiguration _configuration;
        private readonly ContextoGeneral _context;

        public BLConsultar_Detalle_Master(IConfiguration configuration, ContextoGeneral context)
        {
            _configuration = configuration;
            _context = context;
        }
        public async Task<List<Detalle_MasterDto>> ListaDetalle(int id)
        {
            try
            {
                var resultado =  await  _context.DETALLE_MASTER
                                        .Where(m => m.ID_TIPO == id && m.Habilitado)
                                        .Select(m => new Detalle_MasterDto
                                        {
                                            Id = m.Id,
                                            Nombre = m.Nombre,
                                            Abreviacion = m.Abreviacion,
                                            Descripcion = m.Descripcion,
                                            IdTipo = m.ID_TIPO,
                                            Habilitado = m.Habilitado
                                        })
                                    .ToListAsync();

                return resultado;
            }
            catch (Exception ex)
            {
                return new List<Detalle_MasterDto> ();
            }
        }
    }
}
