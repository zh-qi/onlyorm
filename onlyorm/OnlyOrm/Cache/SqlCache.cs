using System;
using System.Reflection;
using OnlyOrm.Exetnds;
using System.Linq;
using System.Collections.Generic;

namespace OnlyOrm.Cache
{
    /// <summary>
    /// 对Sql语句进行缓存，不必每次都动态生成，提高了效率
    /// 采用泛型+字典双结构缓存
    /// </summary>
    public static class SqlCache<T>where T:OrmBaseModel
    {
        private static Dictionary<string, string> _cacheString = new Dictionary<string, string>();
        static SqlCache()
        {
            InitFindStr();
        }

        public static string GetSql(string sqlType)
        {
            return _cacheString[sqlType];
        }

        private static void InitFindStr()
        {
            Type type = typeof(T);
            string tableName = type.GetMappingName();
            PropertyInfo[] properies = type.GetProperties();
            string primaryKeyStr = type.GetPrimaryKeyStr(properies);
            string columnString = string.Join(",", properies.Select(p => p.GetMappingName()));
            string sqlStr = $"SELECT {columnString} from {tableName} where {primaryKeyStr}=?{primaryKeyStr}";

            _cacheString[SqlType.Find] = sqlStr;
        }
    }
}