using Npgsql;
using pwnctl.app;

namespace pwnctl.infra.Persistence
{
    public sealed class QueryRunner
    {
        public async Task<(bool, string)> TryRunAsync(string sql)
        {
            try
            {
                using (var connection = new NpgsqlConnection(PwnctlDbContext.ConnectionString))
                {
                    var command = new NpgsqlCommand(sql, connection);

                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    var rows = Serialize(reader);
                    var json = PwnInfraContext.Serializer.Serialize(rows);
                    await reader.CloseAsync();

                    return (true, json);
                }
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Warning($"Failed to execute query: {sql}");
                PwnInfraContext.Logger.Exception(ex);
                return (false, "");
            }
        }

        public async Task<string> RunAsync(string sql)
        {
            using (var connection = new NpgsqlConnection(PwnctlDbContext.ConnectionString))
            {
                var command = new NpgsqlCommand(sql, connection);

                await connection.OpenAsync();
                NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                var rows = Serialize(reader);
                var json = PwnInfraContext.Serializer.Serialize(rows);
                await reader.CloseAsync();
                
                return json;
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
