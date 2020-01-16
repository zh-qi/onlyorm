using System.Linq.Expressions;
using MySql.Data.MySqlClient;

namespace OnlyOrm.Exetnds
{
    /// <summary>
    /// 拼接Sql自定义委托
    /// </summary>
    /// <param name="paraName">要拼接的参数名</param>
    /// <param name="method">调用的方法MethodCallExpression对象</param>
    /// <param name="conditonStr">out要拼接的字符串</param>
    /// <returns>参数化后的参数列表</returns>
    internal delegate MySqlParameter[] SqlProcessDelegate(string paraName, MethodCallExpression method, out string conditonStr);
}