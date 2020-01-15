using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using OnlyOrm.Exceptions;

namespace OnlyOrm.Pool
{
    /// <summary>
    /// 数据库连接池,暂时不启用
    /// </summary>
    internal static class ConnectionPool
    {
        private static int _poolSize { get; set; }
        private static string _conectStr { get; set; }
        private static string _sqlType { get; set; }
        private static List<MySqlConnection> _connections = new List<MySqlConnection>();

        private static object _lockObj = new object();
        static ConnectionPool()
        {
            var builder = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json");
            var config = builder.Build();

            _conectStr = config["OnlyOrm:ConnectString"];
            _sqlType = config["OnlyOrm:Type"];
            _poolSize = int.Parse(config["OnlyOrm:ConnectionPoolSize"]);
        }

        public static MySqlConnection GetConnection()
        {
            lock (_lockObj)
            {
                if (_connections.Count > 0)
                {
                    Console.WriteLine("使用第0个链接");
                    var tempConnection = _connections[0];
                    _connections.RemoveAt(0);
                    return tempConnection;
                }

                var conn = CreateConnection();
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}open:目前一共有{_connections.Count}个链接");
                return conn;
            }
        }

        static MySqlConnection CreateConnection()
        {
            MySqlConnection conn = new MySqlConnection(_conectStr);
            conn.Open();
            return conn;
        }

        public static void CloseConnection(MySqlConnection con)
        {
            lock (_lockObj)
            {
                if (null == con)
                    return;

                if (con.State != ConnectionState.Closed)
                {
                    if (_connections.Count < _poolSize)
                    {
                        _connections.Add(con);
                        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}close:目前一共有{_connections.Count}个链接");
                    }
                }
            }
        }
    }
}