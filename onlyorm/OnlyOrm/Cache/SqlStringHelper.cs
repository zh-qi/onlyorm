using System;
using System.Linq;
using System.Reflection;
using OnlyOrm.Exetnds;

namespace OnlyOrm.Cache
{
    /// <summary>
    /// 对生成Sql的方法进行封装，所有的实体类都会从这里生成SQL
    /// </summary>
    internal static class SqlStringHelper
    {
        internal static string GetFindSql(string tableName, PropertyInfo primaryKeyProp, PropertyInfo[] properties)
        {
            var columnString = string.Join(",", properties.Select(p => p.GetMappingName()));
            var sqlStr = $"SELECT {columnString} from {tableName} where {primaryKeyProp.GetMappingName()}=?{primaryKeyProp.GetMappingName()}";
            return sqlStr;
        }

        internal static string GetFindWhereSql(string tableName, PropertyInfo[] properties)
        {
            var columnString = string.Join(",", properties.Select(p => p.GetMappingName()));
            var sqlStr = $"SELECT {columnString} from {tableName} where ";
            return sqlStr;
        }

        internal static string GetInsertSql(string tableName, PropertyInfo[] properties)
        {
            var values = string.Join(",", properties.Select(p => p.GetMappingName()));
            var parameterStr = string.Join(",", properties.Select(p => "?" + p.GetMappingName()));
            var sqlStr = $"INSERT INTO {tableName} ({values}) values ({parameterStr})";

            return sqlStr;
        }

        internal static string GetUpdateSql(string tableName, PropertyInfo[] properties)
        {
            var updateStr = String.Join(",", properties.Select(p => $"{p.GetMappingName()}=?{p.GetMappingName()}"));
            var sqlStr = $"UPDATE {tableName} set {updateStr} Where Id = ?Id";

            return sqlStr;
        }

        internal static string GetDelSql(string tableName, PropertyInfo primaryKeyProp)
        {
            var sqlStr = $"DELETE FROM {tableName} Where {primaryKeyProp.GetMappingName()}=?{primaryKeyProp.GetMappingName()}";
            return sqlStr;
        }
    }
}