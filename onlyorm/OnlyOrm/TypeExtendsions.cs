using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace OnlyOrm
{
    ///<summary>
    /// 实体类的拓展泛型方法，可以通过泛型进行操作ORM
    ///</summary>
    public static class Orm
    {
        private static string _connctStr;
        private static string _sqlType;
        static Orm()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var config = builder.Build();
            
            _connctStr = config["OnlyOrm:ConnectString"];
            _sqlType = config["OnlyOrm:Type"];
        }

        ///<summary>
        /// 按照主键进行查找对应的数据库数据
        ///</summary>
        public static T Find<T>(int key) where T:OnlyOrmBaseModel
        {
            return default(T);
        }
    }
}
