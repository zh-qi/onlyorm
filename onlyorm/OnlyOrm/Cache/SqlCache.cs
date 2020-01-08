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
    public static class SqlCache<T> where T : OrmBaseModel
    {
        private static string TableName { get; set; }
        private static PropertyInfo PrimaryKeyProp { get; set; }
        private static PropertyInfo[] Properties { get; set; }
        private static Dictionary<string, string> _cache = new Dictionary<string, string>();

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

            InitFindStr();
            InitInsertStr();
            InitUpdateStr();
            InitDelStr();
        }

        /// <summary>
        /// 获取缓存的SQL
        /// </summary>
        public static string GetSql(string sqlType)
        {
            return _cache[sqlType];
        }

        /// <summary>
        /// 获取针对按主键查询的MySqlParameter数组
        /// </summary>
        /// <param name="sqlType">Sql类型</param>
        /// <param name="primaryValue">主键的值</param>
        public static MySqlParameter[] GetFindMySqlParameter(string primaryValue)
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
        public static MySqlParameter[] GetInsertMySqlParameters(T t)
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
        public static MySqlParameter[] GetUpdateMySqlParameters(T t)
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
        public static MySqlParameter[] GetDelMySqlParameters(string primaryValue)
        {
            MySqlParameter[] parameters = new[]
            {
                new MySqlParameter($"?{PrimaryKeyProp.GetMappingName()}", primaryValue),
            };

            return parameters;
        }

        private static void InitFindStr()
        {
            var columnString = string.Join(",", Properties.Select(p => p.GetMappingName()));
            var sqlStr = $"SELECT {columnString} from {TableName} where {PrimaryKeyProp.GetMappingName()}=?{PrimaryKeyProp.GetMappingName()}";
            _cache[SqlType.Find] = sqlStr;
        }

        private static void InitInsertStr()
        {
            var values = string.Join(",", Properties.Select(p => p.GetMappingName()));
            var parameterStr = string.Join(",", Properties.Select(p => "?" + p.GetMappingName()));
            var sqlStr = $"INSERT INTO {TableName} ({values}) values ({parameterStr})";

            _cache[SqlType.Insert] = sqlStr;
        }

        private static void InitUpdateStr()
        {
            var updateStr = String.Join(",", Properties.Select(p => $"{p.GetMappingName()}=?{p.GetMappingName()}"));
            var sqlStr = $"UPDATE {TableName} set {updateStr} Where Id = ?Id";

            _cache[SqlType.Update] = sqlStr;
        }

        private static void InitDelStr()
        {
            var sqlStr = $"DELETE FROM {TableName} Where Id=?Id";
            _cache[SqlType.Deleate] = sqlStr;
        }
    }
}