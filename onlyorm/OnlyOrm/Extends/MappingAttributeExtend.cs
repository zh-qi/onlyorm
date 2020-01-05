using System;
using System.Reflection;
using OnlyOrm.Attributes;

namespace OnlyOrm.Exetnds
{
    public static class MappingAttributeExtend
    {
        public static string GetMappingName<T>(this T type) where T: MemberInfo
        {
            if(type.IsDefined(typeof(AbstractMappingAttribute), true))
            {
                AbstractMappingAttribute attribute = type.GetCustomAttribute<AbstractMappingAttribute>();
                return attribute.GetMappingName();
            }

            return type.Name;
        }
    }
}