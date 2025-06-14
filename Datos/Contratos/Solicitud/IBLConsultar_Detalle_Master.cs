using Comun.DTO.Generales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Contratos.Solicitud
{
    public interface IBLConsultar_Detalle_Master
    {
        public Task<List<Detalle_MasterDto>> ListaDetalle(int id);
    }
}
