using System.Linq.Expressions;
using MySql.Data.MySqlClient;

namespace OnlyOrm.Exetnds
{
    /// <summary>
    /// 拼接Sql自定义委托
    /// </summary>
    internal delegate MySqlParameter[] SqlProcessDelegate(string paraName, MethodCallExpression method, out string conditonStr);
}