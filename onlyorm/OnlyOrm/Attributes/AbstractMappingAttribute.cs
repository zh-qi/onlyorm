using System;

namespace OnlyOrm.Attributes
{
    public abstract class AbstractMappingAttribute: System.Attribute
    {
        private string _mappingName;

        public AbstractMappingAttribute(string name)
        {
            this._mappingName = name;
        }

        public string GetMappingName()
        {
            return this._mappingName;
        }
    }
}