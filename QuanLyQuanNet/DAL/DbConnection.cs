using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MySqlConnector;


namespace QuanLyQuanNet.DAL
{
    public class DbConnection
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}