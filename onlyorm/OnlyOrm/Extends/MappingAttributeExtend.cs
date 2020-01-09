using System;
using System.Collections.Generic;
using System.Reflection;
using OnlyOrm.Attributes;
using OnlyOrm.Exceptions;

namespace OnlyOrm.Exetnds
{
    internal static class MappingAttributeExtend
    {
        internal static string GetMappingName<T>(this T type) where T : MemberInfo
        {
            if (type.IsDefined(typeof(AbstractMappingAttribute), true))
            {
                AbstractMappingAttribute attribute = type.GetCustomAttribute<AbstractMappingAttribute>();
                return attribute.GetMappingName();
            }

            return type.Name;
        }

        internal static PropertyInfo GetPrimaryKeyStr(this Type type, PropertyInfo[] props, out bool autoIncr)
        {
            autoIncr = false;
            foreach (var prop in props)
            {
                if (prop.IsDefined(typeof(PrimaryKeyAttribute)))
                {
                    PrimaryKeyAttribute primaryAttribute = prop.GetCustomAttribute<PrimaryKeyAttribute>();
                    autoIncr = primaryAttribute.AutoIncr;

                    AbstractMappingAttribute attribute = prop.GetCustomAttribute<AbstractMappingAttribute>();
                    return prop;
                }
            }

            throw new NoPrimaryException();
        }

        internal static PropertyInfo[] FilterPrimaryKey(this PropertyInfo[] properties)
        {
            var result = new List<PropertyInfo>();
            foreach (var prop in properties)
            {
                if (!prop.IsDefined(typeof(PrimaryKeyAttribute)))
                {
                    result.Add(prop);
                }
            }
            return result.ToArray();
        }
    }
}