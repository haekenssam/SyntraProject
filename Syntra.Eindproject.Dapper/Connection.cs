using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syntra.Eindproject.Dapper
{
    public class Connection
    {
        private static readonly Connection _connection = new Connection();

        public static Connection Instance => _connection;

        private string _connectionString;

        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_connectionString))
                {
                    throw new Exception("Set connectionString first");
                }
                return _connectionString;
            }
            private set => _connectionString = value;
        }

        public void SetConnection(string connectionString)
        {
            if (!string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new Exception("Connection can only be set once");
            }
            ConnectionString = connectionString;
        }

        private Connection() { }
    }
}
