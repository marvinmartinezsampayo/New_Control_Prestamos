﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

        private static bool IsNavigationProperty(PropertyInfo prop)
        {
            // Simple detección de propiedades de navegación
            return prop.PropertyType.IsClass &&
                   prop.PropertyType != typeof(string) &&
                   !prop.PropertyType.IsValueType;
        }

        private static bool IsCompatibleType(Type sourceType, Type targetType)
        {
            // Lógica básica de compatibilidad de tipos
            if (sourceType == targetType) return true;
            if (Nullable.GetUnderlyingType(sourceType) == targetType) return true;
            if (Nullable.GetUnderlyingType(targetType) == sourceType) return true;

            return false;
        }

        // Mapeador en caso de que no haya foto o devulva el arreglo vacio 
        public static TDestination MapearObjetoSeguro<TOrigen, TDestination>(TOrigen origen)
      where TDestination : new()
        {
            var destino = new TDestination();

            foreach (var propDestino in typeof(TDestination).GetProperties())
            {
                var propOrigen = typeof(TOrigen).GetProperty(propDestino.Name);

                if (propOrigen != null && propOrigen.CanRead && propDestino.CanWrite)
                {
                    try
                    {
                        var valor = propOrigen.GetValue(origen);

                        // Verificación robusta para campos byte[]
                        if (valor == DBNull.Value || (valor == null && propDestino.PropertyType == typeof(byte[])))
                        {
                            if (propDestino.PropertyType == typeof(byte[]))
                                propDestino.SetValue(destino, Array.Empty<byte>());
                            else
                                propDestino.SetValue(destino, null);
                        }
                        else
                        {
                            propDestino.SetValue(destino, valor);
                        }
                    }
                    catch
                    {
                        // Ignora errores de asignación
                        continue;
                    }
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
