using Tutorial8.Models.DTOs;
using Tutorial8.trips;

namespace Tutorial8.Services;

public interface ITripsService
{
    Task<List<TripDTO>> GetAllTrips();
    
    Task<List<ClientTripDTO>> GetTripsByClient(int clientId);
    
    Task<TripDTO?> GetTripById(int id);

    Task<bool> Exists(int id);
    
    Task<int> GetTripClientCount(int tripId);

    Task PutClientInTrip(int tripId, int clientId);
    
    Task DeleteClientFromTrip(int tripId, int clientId);
}