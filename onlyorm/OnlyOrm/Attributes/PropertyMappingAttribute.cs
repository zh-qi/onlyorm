using System;
using OnlyOrm.Attributes;

namespace OnlyOrm.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyMappingAttribute :AbstractMappingAttribute
    {
        public PropertyMappingAttribute(string name):base(name)
        {

        }
    }
}