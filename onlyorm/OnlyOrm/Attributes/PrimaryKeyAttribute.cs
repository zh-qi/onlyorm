using System;
using OnlyOrm.Attributes;

namespace OnlyOrm.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute:Attribute
    {
        public bool AutoIncr {get; private set;}
        public PrimaryKeyAttribute(bool autoIncr)
        {
            this.AutoIncr = autoIncr;
        }
    }
}