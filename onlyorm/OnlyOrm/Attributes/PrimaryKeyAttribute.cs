using System;
using OnlyOrm.Attributes;

namespace OnlyOrm.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute:Attribute
    {
        public PrimaryKeyAttribute()
        {

        }
    }
}