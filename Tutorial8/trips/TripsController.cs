using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ITripsService _tripsService;

        public TripsController(ITripsService tripsService)
        {
            _tripsService = tripsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _tripsService.GetAllTrips();

            if (trips.IsNullOrEmpty())
            {
                return Empty;
            }
            
            return Ok(trips);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTrip(int id)
        {
            var trip = await _tripsService.GetTripById(id);

            if (trip == null)
            {
                return NotFound("Trip not found");
            }

            return Ok(trip);
        }
    }
}
