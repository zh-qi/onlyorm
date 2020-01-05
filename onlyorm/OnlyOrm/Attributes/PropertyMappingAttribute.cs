using System;
using OnlyOrm.Attributes;

namespace OnlyOrm.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrppertyMappingAttribute:AbstractMappingAttribute
    {
        public PrppertyMappingAttribute(string name):base(name)
        {

        }
    }
}