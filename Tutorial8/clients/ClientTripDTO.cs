using Tutorial8.Models.DTOs;

namespace Tutorial8.trips;

public class ClientTripDTO
{
   public DateTime RegisteredAt { get; set; }
   public DateTime? PaymentDate { get; set; }
   public TripDTO Trip { get; set; }
}