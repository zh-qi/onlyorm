using System;

namespace OnlyOrm.Exceptions
{
    public class PoolOutOfRangeException : ApplicationException
    {
        public PoolOutOfRangeException() : base("连接池连接数超过最大数")
        {
        }
    }
}