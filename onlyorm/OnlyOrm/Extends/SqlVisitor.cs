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
            if(node.Left.NodeType == ExpressionType.MemberAccess && node.Right.NodeType == ExpressionType.Constant)
            {
                this.ProcessNode(node.NodeType, (ConstantExpression)node.Right, (MemberExpression)node.Left);
                return node;
            }
            if(node.Left.NodeType == ExpressionType.Constant && node.Right.NodeType == ExpressionType.MemberAccess)
            {
                this.ProcessNode(node.NodeType, (ConstantExpression)node.Left, (MemberExpression)node.Right);
                return node;
            }
            this.Visit(node.Left);
            this._conditions.Push(GetSqlOperate(node.NodeType));
            this.Visit(node.Right);

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression method)
        {
            if (null == method)
                throw new ArgumentNullException("method is null");

            string paraValue = (method.Arguments[0] as ConstantExpression).Value.ToString();
            switch (method.Method.Name)
            {
                case "StartWith":
                    paraValue = $"{paraValue}%";
                    break;
                case "Contains":
                    paraValue = $"%{paraValue}%";
                    break;
                case "EndWith":
                    paraValue = $"%{paraValue}";
                    break;
                default:
                    throw new MethodNotSupportException(method.Method.Name);
            }
            var memberNode = method.Object as MemberExpression;
            this._conditions.Push(string.Format("({0} LIKE ?{0})", memberNode.Member.GetMappingName()));
            this._parameters.Add(new MySqlParameter($"?{memberNode.Member.GetMappingName()}", paraValue));
            return method;
        }

        private void ProcessNode(ExpressionType nodeType, ConstantExpression cNode, MemberExpression mNode)
        {
            var memberName = mNode.Member.GetMappingName();
            this._conditions.Push($"{memberName} {GetSqlOperate(nodeType)} ?{memberName}");

            var value = cNode.Value.ToString();
            this._parameters.Add(new MySqlParameter($"?{memberName}", value));
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
    }
}