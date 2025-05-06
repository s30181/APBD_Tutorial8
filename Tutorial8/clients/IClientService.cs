namespace Tutorial8.clients;

public interface IClientService
{
    Task<bool> Exists(int id);
    Task<ClientDTO> Create(ClientCreateDTO createDto);
    Task<bool> IsOnATrip(int clientId, int tripId);
}