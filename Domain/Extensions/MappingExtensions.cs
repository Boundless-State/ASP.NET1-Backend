using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Extentions;

public static class MappingExtensions
{
    public static TDestination MapTo<TDestination>(this object source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        TDestination destination = Activator.CreateInstance<TDestination>();

        var sourceProperties = source.GetType().GetProperties((BindingFlags.Public | BindingFlags.Instance));
        var destinationProperties = destination.GetType().GetProperties((BindingFlags.Public | BindingFlags.Instance));

        foreach (var d_property in destinationProperties)
        {
            var s_property = sourceProperties.FirstOrDefault(x => x.Name == d_property.Name && x.PropertyType == d_property.PropertyType);
            if (s_property != null && d_property.CanWrite)
            {
                var value = s_property.GetValue(source);
                d_property.SetValue(destination, value);
            }
        }
        
        return destination;
    }
}
