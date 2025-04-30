using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;
using Tutorial8.trips;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=localhost, 1433; User=SA; Password=yourStrong(!)Password; Initial Catalog=apbd; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";

    private async Task<List<T>> GetList<T>(string command, Func<SqlDataReader, T> function, Dictionary<string, object>? parameters = null)
    {
        var result = new List<T>();
        
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

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    result.Add(function(reader));
                }
            }
        }

        return result;
    }

    private async Task<TripDTO> GetTripDto(SqlDataReader reader)
    {
        return new TripDTO
        {
            Id = reader.GetInt32(reader.GetOrdinal("IdTrip")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Description = reader.GetString(reader.GetOrdinal("Description")),
            DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
            DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
            MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
            Countries = await GetList(
                "SELECT Name FROM Country_Trip join Country ON Country.IdCountry = Country_Trip.IdCountry where IdTrip = @IdTrip",
                reader => new CountryDTO() { Name = reader.GetString(reader.GetOrdinal("Name")) },
                new Dictionary<string, object>() { { "@IdTrip", reader.GetInt32(reader.GetOrdinal("IdTrip")) } })
        };
    }
    
    public async Task<List<TripDTO>> GetAllTrips()
    {
        var trips = await GetList("SELECT IdTrip, DateFrom, DateTo, Name, Description, MaxPeople FROM Trip", GetTripDto);
        
        return (await Task.WhenAll(trips)).ToList();
    }

    public async Task<List<ClientTripDTO>> GetClientTrips()
    {
        var command = """
                      SELECT IdTrip, DateFrom, DateTo, Name, Description, MaxPeople, RegisteredAt, PaymentDate
                      FROM Trip
                      JOIN Client_Trip ON Client_Trip.IdTrip = Trip.IdTrip
                      JOIN Client ON Client_Trip.IdClient = Client.IdClient
                      where Client.IdClient = @ClientId
                      """;
        
        var trips = await GetList(
            command,
            async reader => new ClientTripDTO {
                Trip = await GetTripDto(reader),
                RegisteredAt = reader.GetDateTime(reader.GetOrdinal("RegisteredAt")),
                PaymentDate = reader.GetDateTime(reader.GetOrdinal("PaymentDate"))
            });
        
        return (await Task.WhenAll(trips)).ToList();
    }
}