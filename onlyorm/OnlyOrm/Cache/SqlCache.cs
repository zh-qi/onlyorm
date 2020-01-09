using System;
using OnlyOrm.Exetnds;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace OnlyOrm.Cache
{
    /// <summary>
    /// 对Sql语句进行缓存，不必每次都动态生成，提高了效率
    /// 采用泛型+字典双结构缓存
    /// </summary>
    internal static class SqlCache<T> where T : OrmBaseModel
    {
        private static string TableName { get; set; }
        internal static PropertyInfo PrimaryKeyProp { get; set; }
        internal static PropertyInfo[] Properties { get; set; }
        private static Dictionary<SqlType, string> _cache = new Dictionary<SqlType, string>();
        internal static PropertyInfo[] AllProperties
        {
            get
            {
                if (null == PrimaryKeyProp)
                    return Properties;

                var list = Properties.ToList();
                list.Add(PrimaryKeyProp);
                return list.ToArray();
            }
        }

        static SqlCache()
        {
            var type = typeof(T);
            TableName = type.GetMappingName();
            Properties = type.GetProperties();
            bool primaryAutoIncr;
            PrimaryKeyProp = type.GetPrimaryKeyStr(Properties, out primaryAutoIncr);
            if (primaryAutoIncr)
            {
                Properties = Properties.FilterPrimaryKey();
            }

            _cache[SqlType.Find] = SqlStringHelper.GetFindSql(TableName, PrimaryKeyProp, AllProperties);
            _cache[SqlType.Insert] = SqlStringHelper.GetInsertSql(TableName, Properties);
            _cache[SqlType.Deleate] = SqlStringHelper.GetDelSql(TableName, PrimaryKeyProp);
            _cache[SqlType.Update] = SqlStringHelper.GetUpdateSql(TableName, Properties);
            _cache[SqlType.FindWhere] = SqlStringHelper.GetFindWhereSql(TableName, AllProperties);
        }

        /// <summary>
        /// 获取缓存的SQL
        /// </summary>
        internal static string GetSql(SqlType sqlType)
        {
            return _cache[sqlType];
        }

        /// <summary>
        /// 获取针对按主键查询的MySqlParameter数组
        /// </summary>
        /// <param name="sqlType">Sql类型</param>
        /// <param name="primaryValue">主键的值</param>
        internal static MySqlParameter[] GetFindMySqlParameter(string primaryValue)
        {
            MySqlParameter[] parameters = new[]
            {
                new MySqlParameter($"?{PrimaryKeyProp.GetMappingName()}", primaryValue),
            };

            return parameters;
        }

        /// <summary>
        /// 获取针对插入的的MySqlParameter数组
        /// </summary>
        /// <param name="sqlType">Sql类型</param>
        /// <param name="primaryValue">实体类的实例</param>
        internal static MySqlParameter[] GetInsertMySqlParameters(T t)
        {
            var parameters = Properties.Select(
                    p => new MySqlParameter($"?{p.GetMappingName()}", p.GetValue(t) ?? DBNull.Value)
                ).ToList();
            if (null == PrimaryKeyProp)
            {
                parameters.Add(new MySqlParameter($"?{PrimaryKeyProp.GetMappingName()}", PrimaryKeyProp.GetValue(t)));
            }

            return parameters.ToArray();
        }

        /// <summary>
        /// 获取针对更新的的MySqlParameter数组
        /// </summary>
        /// <param name="sqlType">Sql类型</param>
        /// <param name="primaryValue">实体类的实例</param>
        internal static MySqlParameter[] GetUpdateMySqlParameters(T t)
        {
            var parameters = Properties.Select(
                    p => new MySqlParameter($"?{p.GetMappingName()}", p.GetValue(t) ?? DBNull.Value)
                ).ToList();

            parameters.Add(new MySqlParameter($"?{PrimaryKeyProp.GetMappingName()}", PrimaryKeyProp.GetValue(t)));
            return parameters.ToArray();
        }

        /// <summary>
        /// 获取针对按主键删除的MySqlParameter数组
        /// </summary>
        /// <param name="sqlType">Sql类型</param>
        /// <param name="primaryValue">主键的值</param>
        internal static MySqlParameter[] GetDelMySqlParameters(string primaryValue)
        {
            MySqlParameter[] parameters = new[]
            {
                new MySqlParameter($"?{PrimaryKeyProp.GetMappingName()}", primaryValue),
            };

            return parameters;
        }
    }
}