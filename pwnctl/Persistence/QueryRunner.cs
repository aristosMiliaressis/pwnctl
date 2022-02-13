using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace pwnctl.Persistence
{
    public class QueryRunner
    {
        private readonly string _connectionString;

        public QueryRunner(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Run(string sql)
        {
            // TODO: maybe split sql on ';' semicolon to execute statements separatly
            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand(sql, connection);
                try
                {
                    connection.Open();
                    SQLiteDataReader reader = command.ExecuteReader();
                    var row = Serialize(reader);
                    string json = JsonConvert.SerializeObject(row, Formatting.Indented);
                    Console.WriteLine(json);
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        
        private IEnumerable<Dictionary<string, object>> Serialize(SQLiteDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++) 
                cols.Add(reader.GetName(i));

            while (reader.Read()) 
                results.Add(SerializeRow(cols, reader));

            return results;
        }
        
        private Dictionary<string, object> SerializeRow(IEnumerable<string> cols, SQLiteDataReader reader) {
            var result = new Dictionary<string, object>();
            foreach (var col in cols) 
                result.Add(col, reader[col]);
            return result;
        }
    }
}
