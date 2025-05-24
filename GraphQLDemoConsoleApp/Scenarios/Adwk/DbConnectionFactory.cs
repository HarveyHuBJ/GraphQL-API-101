using GraphQLDemoConsoleApp.Scenarios.Adwk.Models;
using Dapper;


using System.Data.Common;
using System.Data.Odbc;
using System;

namespace GraphQLDemoConsoleApp.Scenarios.Adwk
{
    public class DbConnectionFactory
    {
        private static readonly string _password =Environment.GetEnvironmentVariable("ADWK_DB_PASSWORD");

        public static string Password
        {
            get
            {
                if (string.IsNullOrEmpty(_password))
                {
                    throw new ArgumentNullException("ADWK_DB_PASSWORD", "Environment variable ADWK_DB_PASSWORD is not set.");
                }
                return _password;
            }
        }
        public static DbConnection GetConnection()
        {

            var connectionString = $"DSN=advworks;UID=CloudSA13a62506;PWD={Password};";
            var connection = new OdbcConnection(connectionString);
            return connection;
        }
    }
}