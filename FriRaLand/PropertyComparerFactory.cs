using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RakuLand {
    public static class PropertyComparerFactory {
        public static IComparer<T> Factory<T>(PropertyDescriptor property, ListSortDirection direction) {
            Type typeFromHandle = typeof(PropertyComparer<,>);
            Type[] typeArguments = new Type[]
            {
                typeof(T),
                property.PropertyType
            };
            Type type = typeFromHandle.MakeGenericType(typeArguments);
            return (IComparer<T>)Activator.CreateInstance(type, new object[]
            {
                property,
                direction
            });
        }
    }
}
