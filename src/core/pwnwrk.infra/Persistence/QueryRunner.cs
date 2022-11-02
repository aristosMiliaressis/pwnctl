using System.Text.Json;
using pwnwrk.infra.Configuration;
using System.Data.SqlClient;
using Npgsql;

namespace pwnwrk.infra.Persistence
{
    public sealed class QueryRunner
    {
        public async Task RunAsync(string sql)
        {
            //TODO: maybe split sql on ';' semicolon to execute statements separatly
            using (var connection = new NpgsqlConnection(PwnContext.Config.Db.ConnectionString))
            {
                var command = new NpgsqlCommand(sql, connection);
                try
                {
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    var row = Serialize(reader);
                    string json = string.Join("\n", row.Select(r => JsonSerializer.Serialize(r)));
                    //string json = JsonSerializer.Serialize(row, Formatting.Indented);
                    Console.WriteLine(json);
                    await reader.CloseAsync();
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
