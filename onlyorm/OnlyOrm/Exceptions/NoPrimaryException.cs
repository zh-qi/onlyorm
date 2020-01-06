using System;

namespace OnlyOrm.Exceptions
{
    public class NoPrimaryException:Exception
    {
        public NoPrimaryException():base("没有设置主键")
        {
        }
    }
}