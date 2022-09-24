using Newtonsoft.Json;
using pwnctl.infra.Configuration;
using System.Data.SqlClient;
using Npgsql;

namespace pwnctl.infra.Persistence
{
    public class QueryRunner
    {
        public void Run(string sql)
        {
            //TODO: maybe split sql on ';' semicolon to execute statements separatly
            using (var connection = new NpgsqlConnection(ConfigurationManager.Config.Db.ConnectionString))
            {
                var command = new NpgsqlCommand(sql, connection);
                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();
                    var row = Serialize(reader);
                    Console.WriteLine(row.Count());
                    string json = string.Join("\n", row.Select(r => JsonConvert.SerializeObject(r, Formatting.Indented)));
                    //string json = JsonConvert.SerializeObject(row, Formatting.Indented);
                    Console.WriteLine(json);
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        
        private IEnumerable<Dictionary<string, object>> Serialize(NpgsqlDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++) 
                cols.Add(reader.GetName(i));

            while (reader.Read()) 
                results.Add(SerializeRow(cols, reader));

            return results;
        }
        
        private Dictionary<string, object> SerializeRow(IEnumerable<string> cols, NpgsqlDataReader reader) 
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols) 
                result.Add(col, reader[col]);
            return result;
        }
    }
}
