using System.Globalization;
using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;
using Tutorial8.trips;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    
    private readonly DatabaseHelper _databaseHelper;

    public TripsService(DatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper;
    }

    private async Task<TripDTO> GetTripDto(SqlDataReader reader)
    {
        return new TripDTO {
            Id = reader.GetInt32(reader.GetOrdinal("IdTrip")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Description = reader.GetString(reader.GetOrdinal("Description")),
            DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
            DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
            MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
            Countries = await _databaseHelper.GetList(
                "SELECT Name FROM Country_Trip join Country ON Country.IdCountry = Country_Trip.IdCountry where IdTrip = @IdTrip", 
                reader => Task.FromResult(new CountryDTO { Name = reader.GetString(reader.GetOrdinal("Name")) }),
                new Dictionary<string, object>() { { "@IdTrip", reader.GetInt32(reader.GetOrdinal("IdTrip")) } })
        };
    }
    
    public async Task<List<TripDTO>> GetAllTrips()
    {
        var trips = await _databaseHelper.GetList(
            "SELECT IdTrip, DateFrom, DateTo, Name, Description, MaxPeople FROM Trip", 
            GetTripDto);
        
        return trips.ToList();
    }

    public async Task<List<ClientTripDTO>> GetTripsByClient(int clientId)
    {
        var command = """
                      SELECT Trip.IdTrip, DateFrom, DateTo, Name, Description, MaxPeople, RegisteredAt, PaymentDate
                      FROM Trip
                      JOIN Client_Trip ON Client_Trip.IdTrip = Trip.IdTrip
                      JOIN Client ON Client_Trip.IdClient = Client.IdClient
                      where Client.IdClient = @ClientId
                      """;
        
        var trips = await _databaseHelper.GetList(
            command,
            async reader =>
            {
                var paymentDateFieldId = reader.GetOrdinal("PaymentDate");
                
                 return new ClientTripDTO
                {
                    Trip = await GetTripDto(reader),
                    RegisteredAt = _databaseHelper.IntToDateTime(reader.GetInt32(reader.GetOrdinal("RegisteredAt"))),
                    PaymentDate = reader.IsDBNull(paymentDateFieldId) ? null : _databaseHelper.IntToDateTime(reader.GetInt32(paymentDateFieldId))
                };
            }, 
            new Dictionary<string, object> { { "@ClientId", clientId } });
        
        return trips.ToList();
    }

    public async Task<bool> Exists(int id)
    {
        return await _databaseHelper.GetScalar<int>("SELECT 1 FROM Trip WHERE IdTrip = @IdTrip", new Dictionary<string, object> { { "@IdTrip", id } }) == 1;
    }

    public async Task<int> GetTripClientCount(int tripId)
    {
        var count = await _databaseHelper.GetScalar<int>("SELECT COUNT(1) FROM Client_Trip WHERE IdTrip = @IdTrip",
            new Dictionary<string, object> { { "@IdTrip", tripId } });
        
        return count ?? throw new Exception("Error getting trip client count");
    }

    public Task<TripDTO?> GetTripById(int id)
    {
        return _databaseHelper.GetRow<TripDTO>(
            "SELECT IdTrip, DateFrom, DateTo, Name, Description, MaxPeople FROM Trip WHERE IdTrip = @IdTrip",
            GetTripDto,
            new Dictionary<string, object> { { "@IdTrip", id } }
        );
    }
    


    public async Task PutClientInTrip(int tripId, int clientId)
    {
        await _databaseHelper.Execute(
            "INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate) VALUES (@IdClient, @IdTrip, @RegisteredAt, @PaymentDate)", 
            new Dictionary<string, object>
            {
                { "@IdClient", clientId },
                { "@IdTrip", tripId },
                { "@RegisteredAt", _databaseHelper.DateTimeToInt(DateTime.Now) },
                { "@PaymentDate", DBNull.Value }
            });
        
    }

    public async Task DeleteClientFromTrip(int tripId, int clientId)
    {
        await _databaseHelper.Execute("DELETE FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip", new Dictionary<string, object> { { "@IdClient", clientId }, { "@IdTrip", tripId } });
    }
}