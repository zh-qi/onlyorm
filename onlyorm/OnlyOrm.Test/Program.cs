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
            var user = Orm.Find<User>(1);
        }
    }

    [TableMappingAttribute("user")]
    public class User:OrmBaseModel
    {
        [MasterKeyAttribute]
        [PrppertyMappingAttribute("Id")]
        public int Id{get;set;}

        [PrppertyMappingAttribute("Name")]
        public string Name {get;set;}

        [PrppertyMappingAttribute("Email")]
        public string Email {get;set;}

        [PrppertyMappingAttribute("Mobile")]
        public string Mobile {get;set;}
    }
}
