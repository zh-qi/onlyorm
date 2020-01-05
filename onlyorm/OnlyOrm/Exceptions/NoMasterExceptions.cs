using System;

namespace OnlyOrm.Exceptions
{
    public class NoMasterExceptions:Exception
    {
        public NoMasterExceptions():base("没有设置主键")
        {
        }
    }
}