using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OnlyOrm.Exceptions;
using MySql.Data.MySqlClient;
using System.Reflection;

namespace OnlyOrm.Exetnds
{
    internal class SqlVisitor : ExpressionVisitor
    {
        private Stack<string> _conditions = new Stack<string>();

        private List<MySqlParameter> _parameters = new List<MySqlParameter>();

        public string GetSql()
        {
            string sql = string.Join(" ", _conditions);
            _conditions.Clear();
            return sql;
        }

        public MySqlParameter[] GetParameters()
        {
            return _parameters.ToArray();
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var right = node.Right;
            this.Visit(right);

            var left = node.Left;
            this.Visit(left);

            if (node.Left.NodeType == ExpressionType.MemberAccess)
            {
                if (node.Right.NodeType == ExpressionType.Constant)
                {
                    var leftNode = ((MemberExpression)node.Left);
                    var rightNode = ((ConstantExpression)node.Right);
                    this._conditions.Push("?" + leftNode.Member.GetMappingName());
                    this._conditions.Push(GetSqlOperate(node.NodeType));
                    this._conditions.Push(leftNode.Member.GetMappingName());

                    var memberName = leftNode.Member.GetMappingName();
                    var value = rightNode.Value.ToString();
                    this._parameters.Add(new MySqlParameter($"?{memberName}", value));
                }
            }

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            // this._conditions.Push(node.Member.GetMappingName());
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            // this._conditions.Push(node.Value.ToString());
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression method)
        {
            if (null == method)
                throw new ArgumentNullException("method is null");

            string format;
            switch (method.Method.Name)
            {
                case "StartWith":
                    format = "( {0} LIKE '{1}%')";
                    break;
                case "Contains":
                    format = "( {0} LIKE '%{1}%')";
                    break;
                case "EndWith":
                    format = "({0} LIKE '%{1}')";
                    break;
                default:
                    throw new MethodNotSupportException(method.Method.Name);
            }
            var memberNode = method.Object as MemberExpression;
            this.Visit(method.Object);
            this.Visit(method.Arguments[0]);
            var right = this._conditions.Pop();
            var left = this._conditions.Pop();
            this._conditions.Push(string.Format(format, left, $"?{left}"));
            this._parameters.Add(new MySqlParameter($"?{left}", right));
            return method;
        }

        private static string GetSqlOperate(ExpressionType type)
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

        public static object GetValue(MemberExpression node)
        {
            object value = new object();
            var @object =
              ((ConstantExpression)(node.Expression)).Value; //这个是重点

            if (node.Member.MemberType == MemberTypes.Field)
            {
                value = ((FieldInfo)node.Member).GetValue(@object);
            }
            else if (node.Member.MemberType == MemberTypes.Property)
            {
                value = ((PropertyInfo)node.Member).GetValue(@object);
            }
            return value;
        }
    }
}