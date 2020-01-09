using System;

namespace OnlyOrm.Exceptions
{
    public class OperateNotSupportException : ApplicationException
    {
        public OperateNotSupportException(string oprtate) : base("未支持的操作符:" + oprtate)
        {
        }
    }
}