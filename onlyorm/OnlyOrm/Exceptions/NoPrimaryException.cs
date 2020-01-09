using System;

namespace OnlyOrm.Exceptions
{
    public class NoPrimaryException : ApplicationException
    {
        public NoPrimaryException() : base("没有设置主键")
        {
        }
    }
}