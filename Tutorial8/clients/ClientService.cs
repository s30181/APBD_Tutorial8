namespace Tutorial8.clients;

public class ClientService : IClientService
{
    private readonly DatabaseHelper _databaseHelper;

    public ClientService(DatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper;
    }
    
    public async Task<bool> Exists(int id)
    {
        var isPresent = await _databaseHelper.GetScalar<int>("SELECT 1 FROM Client WHERE Client.IdClient = @id", new Dictionary<string, object> { { "@id", id } });
        return isPresent == 1;
    }

    public async Task<ClientDTO> Create(ClientCreateDTO createDto)
    {
        var id = await _databaseHelper.GetScalar<int>(
            "INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel) VALUES (@firstName, @lastName, @email, @phone, @pesel); SELECT SCOPE_IDENTITY();", 
            new Dictionary<string, object>
            {
                { "@firstName", createDto.FirstName },
                { "@lastName", createDto.LastName },
                { "@email", createDto.Email },
                { "@phone", createDto.Telephone },
                { "@pesel", createDto.Pesel }
            });
        
        return new ClientDTO { Id = id ?? throw new Exception("Could not create client") };
    }

    public async Task<bool> IsOnATrip(int clientId, int tripId)
    {
        var isOnATrip = await _databaseHelper.GetScalar<int>("SELECT 1 FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip", new Dictionary<string, object> { { "@IdClient", clientId }, { "@IdTrip", tripId } });
        
        return isOnATrip == 1;
    }
}