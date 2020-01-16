using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OnlyOrm.Exceptions;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.Collections;
using System.Collections.ObjectModel;

namespace OnlyOrm.Exetnds
{
    /// <summary>
    /// 将表达式目录树解析为对应的SQL
    /// </summary>
    internal class SqlVisitor : ExpressionVisitor
    {
        private Stack<string> _conditions = new Stack<string>();

        private List<MySqlParameter> _parameters = new List<MySqlParameter>();

        /// <summary>
        /// 获取拼接后的SQL
        /// </summary>
        public string GetSql()
        {
            string sql = string.Join(" ", _conditions);
            _conditions.Clear();
            return sql;
        }

        /// <summary>
        /// 获取拼接后的参数化参数
        /// </summary>
        public MySqlParameter[] GetParameters()
        {
            var result = _parameters.ToArray();
            _parameters.Clear();
            return result;
        }

        /// <summary>
        /// 设置要翻译的表达式目录树
        /// </summary>
        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.Left.NodeType == ExpressionType.MemberAccess && node.Right.NodeType == ExpressionType.Constant)
            {
                this.ProcessNode(node.NodeType, (ConstantExpression)node.Right, (MemberExpression)node.Left);
                return node;
            }
            if (node.Left.NodeType == ExpressionType.Constant && node.Right.NodeType == ExpressionType.MemberAccess)
            {
                this.ProcessNode(node.NodeType, (ConstantExpression)node.Left, (MemberExpression)node.Right);
                return node;
            }
            this.Visit(node.Left);
            this._conditions.Push(MontageSqlHelper.GetSqlOperate(node.NodeType));
            this.Visit(node.Right);

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            return node;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (null != node.Body)
            {
                this.Visit(node.Body);
                return node.Body;
            }

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

            switch (method.Method.Name.ToLower())
            {
                case "startwith":
                    ProcessMethodExpress(method, MontageSqlHelper.GetStartWithQueryParaValue);
                    break;
                case "contains":
                    ProcessMethodExpress(method, MontageSqlHelper.GetContainsQueryConditon);
                    break;
                case "endwith":
                    ProcessMethodExpress(method, MontageSqlHelper.GetEndWithQueryConditon);
                    break;
                case "inlist":
                    ProcessMethodExpress(method, MontageSqlHelper.ProcessInListMethod);
                    break;
                case "concat":
                    ProcessMethodExpress(method, MontageSqlHelper.ProcessConcatMethod);
                    break;
                default:
                    throw new MethodNotSupportException(method.Method.Name);
            }

            return method;
        }

        private void ProcessMethodExpress(MethodCallExpression method, SqlProcessDelegate GetQueryParaValue)
        {
            var memberNode = method.Object as MemberExpression;
            var paraName = null == memberNode ? "" : memberNode.Member.GetMappingName();
            string conditionStr = "";
            var parameters = GetQueryParaValue.Invoke(paraName, method, out conditionStr);
            this._conditions.Push(conditionStr);
            this._parameters.AddRange(parameters);
        }

        private void ProcessNode(ExpressionType nodeType, ConstantExpression cNode, MemberExpression mNode)
        {
            var memberName = mNode.Member.GetMappingName();
            this._conditions.Push($"{memberName} {MontageSqlHelper.GetSqlOperate(nodeType)} ?{memberName}");

            var value = cNode.Value.ToString();
            this._parameters.Add(new MySqlParameter($"?{memberName}", value));
        }
    }
}