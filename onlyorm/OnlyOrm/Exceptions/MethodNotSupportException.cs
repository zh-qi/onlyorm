using System;

namespace OnlyOrm.Exceptions
{
    public class MethodNotSupportException : ApplicationException
    {
        public MethodNotSupportException(string oprtate) : base("未支持的操作符:" + oprtate)
        {
        }
    }
}