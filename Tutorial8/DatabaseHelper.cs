using System.Globalization;
using Microsoft.Data.SqlClient;

namespace Tutorial8;

public class DatabaseHelper
{
    private readonly string _connectionString = "Data Source=localhost, 1433; User=SA; Password=yourStrong(!)Password; Initial Catalog=apbd; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;MultipleActiveResultSets=True";

    public async Task<List<T>> GetList<T>(string command, Func<SqlDataReader, Task<T>> function, Dictionary<string, object>? parameters = null)
    {
        var result = new List<T>();

        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(command, conn);
        await conn.OpenAsync();

        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }
        }

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(await function(reader));
        }

        return result;
    }

    public async Task<T?> GetScalar<T>(string command, Dictionary<string, object>? parameters = null) where T : struct
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
            }

            var result = await cmd.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
            {
                return null;
            }

            return (T)Convert.ChangeType(result, typeof(T));
        }
    }
    
    public async Task<T?> GetRow<T>(
        string command, 
        Func<SqlDataReader, Task<T>> function, 
        Dictionary<string, object>? parameters = null)
    where T : class
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(command, conn);
        await conn.OpenAsync();

        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }
        }

        var result = await cmd.ExecuteReaderAsync();
        if (!result.HasRows)
        {
            return null;
        }

        await result.ReadAsync();

        return await function(result);
    }

    public async Task Execute(string command, Dictionary<string, object>? parameters = null)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(command, conn);
        await conn.OpenAsync();

        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }
        }

        var result = await cmd.ExecuteNonQueryAsync();
        
        return;
    }
    
    public DateTime IntToDateTime(int value)
    {
        return DateTime.ParseExact(value.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
    }

    public int DateTimeToInt(DateTime value)
    {
        return int.Parse(value.ToString("yyyyMMdd"));
    }

}