using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Tutorial8.clients;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly ITripsService _tripsService;
    private readonly IClientService _clientService;
    private readonly ClientValidator _clientValidator;

    public ClientsController(ITripsService tripsService, IClientService clientService, ClientValidator clientValidator)
    {
        _tripsService = tripsService;
        _clientService = clientService;
        _clientValidator = clientValidator;
    }
    
    [HttpGet("{id:int}/trips")]
    public async Task<IActionResult> GetClientTrips(int id)
    {
        if (!await _clientService.Exists(id))
        {
            return NotFound("Client not found");
        }
        
        var trips = await _tripsService.GetTripsByClient(id);

        if (trips.IsNullOrEmpty())
        {
            return Empty;
        }
        
        return Ok(trips);
    }
    
    [HttpPost("")]
    public async Task<IActionResult> CreateNewClient(ClientCreateDTO createDto)
    {
        try
        {
            await _clientValidator.Validate(createDto);
            var dto = await _clientService.Create(createDto);

            return Created($"/api/clients/{dto.Id}", dto);
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("{id:int}/trips/{tripId:int}")]
    public async Task<IActionResult> PutClientInTrip(int id, int tripId)
    {
        var trip = await _tripsService.GetTripById(tripId);
        if (trip == null)
        {
            return NotFound("Trip not found");
        }

        if (!await _clientService.Exists(id))
        {
            return NotFound("Client not found");
        }

        var currentPeopleAmount = await _tripsService.GetTripClientCount(tripId);
        if (currentPeopleAmount >= trip.MaxPeople)
        {
            return BadRequest("Trip is full");
        }

        var isCurrentlyOnATrip = await _clientService.IsOnATrip(id, tripId);
        if (isCurrentlyOnATrip)
        {
            return BadRequest("Client is already on a trip");
        }
        
        await _tripsService.PutClientInTrip(tripId, id);
                
        return NoContent();
    }

    [HttpDelete("{id:int}/trips/{tripId:int}")]
    public async Task<IActionResult> DeleteClientFromTrip(int id, int tripId)
    {
        if (!await _clientService.Exists(id))
        {
            return NotFound("Client not found");
        }
        
        if (!await _tripsService.Exists(tripId))
        {
            return NotFound("Trip not found");
        }
        
        if (!await _clientService.IsOnATrip(id, tripId))
        {
            return BadRequest("Client is not on a trip");
        }

        await _tripsService.DeleteClientFromTrip(tripId, id);

        return NoContent();
    }
}