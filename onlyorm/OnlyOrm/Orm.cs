using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using MySql.Data.MySqlClient;
using OnlyOrm.Exetnds;

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
        public static T Find<T>(int key) where T:OrmBaseModel
        {
            Type type = typeof(T);
            string tableName = type.GetMappingName();
            return default(T);
        }

        private static T ExceteSql<T>(string sqlStr, MySqlParameter[] parameters, Func<MySqlCommand, T> callback)
        { 
            using(MySqlConnection conn = new MySqlConnection(_connctStr))
            {
                MySqlCommand command = new MySqlCommand(_connctStr, conn);
                command.Parameters.AddRange(parameters);
                conn.Open();
                return callback.Invoke(command);
            }
        }
    }
}
