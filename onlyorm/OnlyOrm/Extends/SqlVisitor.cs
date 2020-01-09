using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OnlyOrm.Exceptions;

namespace OnlyOrm.Exetnds
{
    internal class SqlVisitor : ExpressionVisitor
    {
        private Stack<string> _conditions = new Stack<string>();

        public string GetSql()
        {
            string sql = string.Join(" ", _conditions);
            _conditions.Clear();
            return sql;
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var right = node.Right;
            this.Visit(right);
            this._conditions.Push(GetSqlOperate(node.NodeType));
            var left = node.Left;
            this.Visit(left);
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            this._conditions.Push(node.Member.GetMappingName());
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            this._conditions.Push(node.Value.ToString());
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
            this.Visit(method.Object);
            this.Visit(method.Arguments[0]);
            var right = this._conditions.Pop();
            var left = this._conditions.Pop();
            this._conditions.Push(string.Format(format, left, right));
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
                default:
                    throw new OperateNotSupportException(type.ToString());
            }
        }
    }
}