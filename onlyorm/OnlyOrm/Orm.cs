using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;
using MySql.Data.MySqlClient;
using OnlyOrm.Exetnds;
using System.Reflection;
using OnlyOrm.Cache;

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
        public static T Find<T>(string key) where T:OrmBaseModel
        {
            Type type = typeof(T);
            PropertyInfo[] properies = type.GetProperties();
            string primaryKeyStr = type.GetPrimaryKeyStr(properies);
            string sqlStr = SqlCache<T>.GetSql(SqlType.Find);
            MySqlParameter[] parameters = new []
            {
                new MySqlParameter(primaryKeyStr, key.ToString()),
            };

            return ExceteSql<T>(sqlStr, parameters, command =>
            {
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    T result = Activator.CreateInstance<T>();
                    foreach (var proerty in properies)
                    {
                        var value = reader[proerty.GetMappingName()];
                        proerty.SetValue(result, value is DBNull ? null : value);
                    }
                    return result;
                }

                return default(T);
            });
        }

        private static T ExceteSql<T>(string sqlStr, MySqlParameter[] parameters, Func<MySqlCommand, T> callback)
        { 
            using(MySqlConnection conn = new MySqlConnection(_connctStr))
            {
                MySqlCommand command = new MySqlCommand(sqlStr, conn);
                command.Parameters.AddRange(parameters);
                conn.Open();
                return callback.Invoke(command);
            }
        }
    }
}
