using System;
using System.Collections.Generic;
using OnlyOrm.Attributes;

namespace OnlyOrm.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            {
                // 查找主键为1的用户
                var user1 = Orm.Find<SuperUser>("1");
            }
            {
                // 组合条件查询，暂时不支持同个条件的或，比如：（2 == u.Id || u.Id == 3）
                // var user1 =Orm.FindWhere<SuperUser>(u=>u.Id == 2 || u.Id == 3);
                var user = Orm.FindWhere<SuperUser>(u => (u.NickName.Contains("zhang") || 2 == u.Id));
                Console.WriteLine(user.Count);
            }
            {
                // 插入一个User实例，如果Id是主键，会被过滤掉
                var user1 = new SuperUser
                {
                    Id = 3,
                    NickName = "test",
                    Mobile = "456",
                    Email = "344@qq.com"
                };

                Orm.Insert<SuperUser>(user1);
            }
            {
                // 更新主键是3dUser
                var user1 = new SuperUser
                {
                    Id = 3,
                    NickName = "test1",
                    Mobile = "456",
                    Email = "344@qq.com"
                };
                Orm.Update<SuperUser>(user1);
            }
            {
                // 删除主键是3的User
                // Orm.Deleate<User>("3");
            }
        }
    }
}
