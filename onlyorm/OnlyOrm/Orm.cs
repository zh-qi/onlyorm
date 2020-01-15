using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using System.IO;
using MySql.Data.MySqlClient;
using OnlyOrm.Exetnds;
using OnlyOrm.Pool;
using System.Reflection;
using OnlyOrm.Cache;
using System.Collections.Generic;
using System.Data;

namespace OnlyOrm
{
    ///<summary>
    /// 实体类的拓展泛型方法，可以通过泛型进行操作ORM，，
    /// 暂时不支持联表查询
    /// 暂时不支持多个或语句,比如：Orm.FindWhere<SuperUser>(u=>u.Id == 2 || u.Id == 3);因为暂时没法把表达式目录树转为in语句
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
        public static T Find<T>(string key) where T : OrmBaseModel
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("primary key can't be null");

            var sqlStr = SqlCache<T>.GetSql(SqlType.Find);
            var parameters = SqlCache<T>.GetFindMySqlParameter(key);

            return ExceteSql<T>(sqlStr, parameters, command =>
            {
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Type type = typeof(T);
                    PropertyInfo[] properies = SqlCache<T>.AllProperties;
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

        /// <summary>
        /// 批量获取符合条件的数据
        /// </summary>
        public static IList<T> FindWhere<T>(Expression<Func<T, bool>> conditions) where T : OrmBaseModel
        {
            SqlVisitor visitor = new SqlVisitor();
            visitor.Visit(conditions);
            var sqlStr = SqlCache<T>.GetSql(SqlType.FindWhere) + visitor.GetSql();
            var parameters = visitor.GetParameters();
            return ExceteSql<IList<T>>(sqlStr, parameters, command =>
            {
                var result = new List<T>();
                var type = typeof(T);
                var properies = SqlCache<T>.AllProperties;
                var adapter = new MySqlDataAdapter(command);
                var dataSet = new DataSet();
                var count = adapter.Fill(dataSet);

                for (var i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    T t = Activator.CreateInstance<T>();
                    foreach (var proerty in properies)
                    {
                        var value = dataSet.Tables[0].Rows[i][proerty.GetMappingName()];
                        proerty.SetValue(t, value is DBNull ? null : value);
                    }
                    result.Add(t);
                }

                return result;
            });
        }

        /// <summary>
        /// 插入实体，如果实体的主键是自动增长的，会被过滤掉
        /// </summary>
        public static bool Insert<T>(T t) where T : OrmBaseModel
        {
            if (null == t)
                throw new ArgumentNullException("instance can't be null");

            var parameters = SqlCache<T>.GetInsertMySqlParameters(t);
            var sqlStr = SqlCache<T>.GetSql(SqlType.Insert);

            return ExceteSql<bool>(sqlStr, parameters, command =>
            {
                var result = command.ExecuteNonQuery();
                return result == 1;
            });
        }

        /// <summary>
        /// 根据实例主键进行更新
        /// </summary>
        public static bool Update<T>(T t) where T : OrmBaseModel
        {
            if (null == t)
                throw new ArgumentNullException("instance can't be null");

            var parameters = SqlCache<T>.GetUpdateMySqlParameters(t);
            var sqlStr = SqlCache<T>.GetSql(SqlType.Update);

            return ExceteSql<bool>(sqlStr, parameters, command =>
            {
                var result = command.ExecuteNonQuery();
                return result == 1;
            });
        }

        /// <summary>
        /// 按主键值进行删除
        /// </summary>
        public static bool Deleate<T>(string key) where T : OrmBaseModel
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("primary key can't be null");

            var sqlStr = SqlCache<T>.GetSql(SqlType.Deleate);
            var parameters = SqlCache<T>.GetDelMySqlParameters(key);

            return ExceteSql<bool>(sqlStr, parameters, command =>
            {
                var result = command.ExecuteNonQuery();
                return result == 1;
            });
        }

        private static T ExceteSql<T>(string sqlStr, MySqlParameter[] parameters, Func<MySqlCommand, T> callback)
        {
            var connection = ConnectionPool.GetConnection();
            using (MySqlConnection conn = new MySqlConnection(_connctStr))
            {
                MySqlCommand command = new MySqlCommand(sqlStr, conn);
                if (null != parameters)
                {
                    command.Parameters.AddRange(parameters);
                }
                conn.Open();
                return callback.Invoke(command);
            }
        }
    }
}
