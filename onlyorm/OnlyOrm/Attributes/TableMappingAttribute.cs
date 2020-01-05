using System;
using OnlyOrm.Attributes;

namespace OnlyOrm.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableMappingAttribute:AbstractMappingAttribute
    {
        public TableMappingAttribute(string name):base(name)
        {

        }
    }
}