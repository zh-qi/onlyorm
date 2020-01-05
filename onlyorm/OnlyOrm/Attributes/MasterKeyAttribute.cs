using System;
using OnlyOrm.Attributes;

namespace OnlyOrm.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MasterKeyAttribute:Attribute
    {
        public MasterKeyAttribute()
        {

        }
    }
}