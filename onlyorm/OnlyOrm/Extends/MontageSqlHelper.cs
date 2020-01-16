using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using MySql.Data.MySqlClient;
using OnlyOrm.Exceptions;

namespace OnlyOrm.Exetnds
{
    /// <summary>
    /// 拼接Sql帮助类
    /// </summary>
    internal static class MontageSqlHelper
    {

        internal static string SqlPrifix { get; set; }

        /// <summary>
        /// 策略方法：获取StartWith方法的参数和拼接字符串
        /// </summary>
        internal static MySqlParameter[] GetStartWithQueryParaValue(string paraName, MethodCallExpression method, out string conditionStr)
        {
            conditionStr = $"({paraName} LIKE {SqlPrifix}{paraName})";
            var result = new MySqlParameter[]
            {
                new MySqlParameter(paraName, $"{(method.Arguments[0] as ConstantExpression).Value.ToString()}%")
            };
            return result;
        }

        /// <summary>
        /// 策略方法：获取EndWith方法的参数和拼接字符串
        /// </summary>
        internal static MySqlParameter[] GetEndWithQueryConditon(string paraName, MethodCallExpression method, out string conditionStr)
        {
            conditionStr = $"({paraName} LIKE {SqlPrifix}{paraName})";
            var result = new MySqlParameter[]
            {
                new MySqlParameter(paraName,$"%{(method.Arguments[0] as ConstantExpression).Value.ToString()}")
            };
            return result;
        }
        /// <summary>
        /// 策略方法：获取Contains方法的参数和拼接字符串
        /// </summary>
        internal static MySqlParameter[] GetContainsQueryConditon(string paraName, MethodCallExpression method, out string conditionStr)
        {
            conditionStr = $"({paraName} LIKE {SqlPrifix}{paraName})";
            var result = new MySqlParameter[]
            {
                new MySqlParameter(paraName,$"%{(method.Arguments[0] as ConstantExpression).Value.ToString()}%")
            };
            return result;
        }

        /// <summary>
        /// 策略方法：获取InList方法的参数和拼接字符串
        /// </summary>
        internal static MySqlParameter[] ProcessInListMethod(string paraName, MethodCallExpression method, out string conditionStr)
        {
            var arg0 = method.Arguments[0] as MemberExpression;
            var arg1 = method.Arguments[1] as MemberExpression;

            var value = (arg1.Expression as ConstantExpression).Value;
            var type = value.GetType();
            var valueList = ((FieldInfo)(arg1.Member)).GetValue(value) as IList;
            var mappingName = arg0.Member.GetMappingName();

            var result = new List<MySqlParameter>();
            var inStr = "";
            for (var i = 0; i < valueList.Count; i++)
            {
                inStr += SqlPrifix + mappingName + i;
                result.Add(new MySqlParameter($"{SqlPrifix}{mappingName + i}", valueList[i].ToString()));
                if (i != valueList.Count - 1)
                {
                    inStr += ",";
                }
            }
            conditionStr = $"( {mappingName} IN ({inStr}) )";

            return result.ToArray();
        }

        /// <summary>
        /// 策略方法：获取ConCat方法的参数和拼接字符串
        /// </summary>
        internal static MySqlParameter[] ProcessConcatMethod(string paraName, MethodCallExpression method, out string conditionStr)
        {
            var arg0 = method.Arguments[0] as MemberExpression;
            var mappingName = arg0.Member.GetMappingName();
            conditionStr = $"{mappingName} =  Concat (";
            var result = GetConCatConditon(method.Arguments, mappingName, ref conditionStr);
            conditionStr += ")";
            return result;
        }

        internal static string GetSqlTypePrix(string sqlType)
        {
            switch (sqlType)
            {
                case "mysql":
                    return "?";
                case "sqlserver":
                    return "@";
                case "oracle":
                    return ":";
                default:
                    return "?";
            }
        }

        internal static string GetSqlOperate(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Equal:
                    return " = ";
                case ExpressionType.GreaterThan:
                    return " > ";
                case ExpressionType.GreaterThanOrEqual:
                    return " >= ";
                case ExpressionType.LessThan:
                    return " < ";
                case ExpressionType.LessThanOrEqual:
                    return " <= ";
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return " OR ";
                case (ExpressionType.Not):
                    return " NOT ";
                default:
                    throw new OperateNotSupportException(type.ToString());
            }
        }
        private static MySqlParameter[] GetConCatConditon(ReadOnlyCollection<Expression> expressions, string mappingName, ref string conditionStr)
        {
            var result = new List<MySqlParameter>();
            for (var i = 0; i < expressions.Count; i++)
            {
                switch (expressions[i].NodeType)
                {
                    case ExpressionType.MemberAccess:
                        conditionStr += (expressions[i] as MemberExpression).Member.GetMappingName();
                        break;
                    case ExpressionType.Constant:
                        var value = (expressions[i] as ConstantExpression).Value;
                        result.Add(new MySqlParameter($"{SqlPrifix}{mappingName + i}", value));
                        conditionStr += $"{SqlPrifix}{mappingName + i}";
                        break;
                    case ExpressionType.NewArrayInit:
                        var ex = (expressions[i] as NewArrayExpression).Expressions;
                        result.AddRange(GetConCatConditon(ex, mappingName, ref conditionStr));
                        break;
                    default:
                        break;
                }

                if (i != expressions.Count - 1)
                {
                    conditionStr += ",";
                }
            }

            return result.ToArray();
        }
    }
}