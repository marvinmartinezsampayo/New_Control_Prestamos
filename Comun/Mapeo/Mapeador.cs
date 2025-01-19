using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Mapeo
{
    public class Mapeador
    {
        public static TDestination MapearObjeto<TOrigen, TDestination>(TOrigen origen)
        {
            var destino = Activator.CreateInstance<TDestination>();
            foreach (var propOrigen in typeof(TOrigen).GetProperties())
            {
                var propDestino = typeof(TDestination).GetProperty(propOrigen.Name);
                if (propDestino != null && propOrigen.CanRead && propDestino.CanWrite)
                {
                    propDestino.SetValue(destino, propOrigen.GetValue(origen));
                }
            }
            return destino;
        }

        public static List<TReturn> ConvertirDataTableAListaDto<TReturn>(DataTable dt)
        {
            const BindingFlags bandera = BindingFlags.Public | BindingFlags.Instance;
            var NombreDeLasColumnas = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();
            var PropiedadesDelObjeto = typeof(TReturn).GetProperties(bandera);
            var ListaDto = dt.AsEnumerable().Select(datosFila =>
            {
                var crearInstancia = Activator.CreateInstance<TReturn>();


                foreach (var propiedad in PropiedadesDelObjeto.Where(propiedades => NombreDeLasColumnas.Contains(propiedades.Name) && datosFila[propiedades.Name] != DBNull.Value))
                {
                    propiedad.SetValue(crearInstancia, datosFila[propiedad.Name], null);
                }
                return crearInstancia;
            }).ToList();


            return ListaDto;
        }

    }
}
