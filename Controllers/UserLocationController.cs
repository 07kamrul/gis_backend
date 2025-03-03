using gis_backend.Models;
using gis_backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gis_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserLocationController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserLocationController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/UserLocation
    [HttpGet("GetAllUserLocations")]
    public async Task<IActionResult> GetAllUserLocations()
    {
        var userLocations = await _context.UserLocations.ToListAsync();
        return Ok(new { Message = "User locations retrieved successfully!", Data = userLocations });
    }

    // GET: api/UserLocation/{id}
    [HttpGet("GetUserLocationByUserId")]
    public async Task<IActionResult> GetUserLocationByUserId(int userId)
    {
        var userLocation = await _context.UserLocations
            .Where(u=> u.UserId == userId)
            .AsNoTracking().FirstOrDefaultAsync();

        if (userLocation == null)
        {
            return NotFound(new { Message = "User location not found." });
        }

        return Ok(new { Message = "User location retrieved successfully!", Data = userLocation });
    }
    

    // PUT: api/UserLocation/{id}
    [HttpPut("UpdateUserLocation")]
    public async Task<IActionResult> UpdateUserLocation(UserLocation userLocation)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { Message = "Invalid data provided.", Errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        var existingUserLocation = await _context.UserLocations.FindAsync(userLocation.Id);
        if (existingUserLocation == null)
        {
            return NotFound(new { Message = "User location not found." });
        }

        existingUserLocation.UserId = userLocation.UserId;
        existingUserLocation.Latitude = userLocation.Latitude;
        existingUserLocation.Longitude = userLocation.Longitude;
        existingUserLocation.IsSharingLocation = userLocation.IsSharingLocation;

        _context.UserLocations.Update(existingUserLocation);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "User location updated successfully!", Data = existingUserLocation });
    }

    // DELETE: api/UserLocation/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserLocation(int id)
    {
        var userLocation = await _context.UserLocations.FindAsync(id);

        if (userLocation == null)
        {
            return NotFound(new { Message = "User location not found." });
        }

        _context.UserLocations.Remove(userLocation);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "User location deleted successfully!" });
    }
}