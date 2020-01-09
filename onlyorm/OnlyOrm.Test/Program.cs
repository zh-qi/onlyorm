using System;
using OnlyOrm.Attributes;

namespace OnlyOrm.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // 查找主键为1的用户
            // var user = Orm.Find<User>("1");

            var user1 = new User
            {
                Id = 3,
                Name = "test",
                Mobile = "456",
                Email = "344@qq.com"
            };

            // Orm.Insert<User>(user1);
            // Orm.Update<User>(user1);

            Orm.Deleate<User>("3");
        }
    }
}
