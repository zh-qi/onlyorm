using System;

namespace OnlyOrm.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var str = Orm.Find<User>(1);
        }
    }

    public class User:OnlyOrmBaseModel
    {

    }
}
