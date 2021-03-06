﻿using System;
using System.Collections.Generic;
using System.Threading;
using OnlyOrm.Attributes;
using OnlyOrm.Exetnds;

namespace OnlyOrm.Test
{
    /// <summary>
    /// ONLYORM单元测试
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            {
                // 查找主键为1的用户
                // var user1 = Orm.Find<SuperUser>("1");
            }
            {
                // 组合条件查询，暂时不支持同个条件的或，比如：（2 == u.Id || u.Id == 3）
                // var user1 =Orm.FindWhere<SuperUser>(u=>u.Id == 2 || u.Id == 3);
                // var user = Orm.FindWhere<SuperUser>(u => (u.NickName.Contains("zhang") || 2 == u.Id));
                // Console.WriteLine(user.Count);
            }
            {
                // 按照表达式目录树的方式进行查询，更新，删除，
                var update = Orm.UpdateWhere<SuperUser>(u => u.NickName.ConCat("222", u.Email),
                                                        u => u.Id == 1);
            }
            {
                // 插入一个User实例，如果Id是主键，会被过滤掉
                // var user1 = new SuperUser
                // {
                //     Id = 3,
                //     NickName = "test",
                //     Mobile = "456",
                //     Email = "344@qq.com"
                // };

                // Orm.Insert<SuperUser>(user1);
            }
            {
                // 更新主键是3的User
                // var user1 = new SuperUser
                // {
                //     Id = 3,
                //     NickName = "test1",
                //     Mobile = "456",
                //     Email = "344@qq.com"
                // };
                // Orm.Update<SuperUser>(user1);
            }
            {
                // 删除主键是3的User
                // Orm.Deleate<User>("3");
            }
        }
        private static void Pool_Test()
        {
            var ids = new List<int>()
                {
                    1,2,3,4,5
                };
            var user = Orm.FindWhere<SuperUser>(u => u.Id.InList<int>(ids));
        }
    }
}
