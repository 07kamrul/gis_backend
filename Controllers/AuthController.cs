using gis_backend.Models;
using gis_backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gis_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        // Validate input
        if (user == null || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
        {
            return BadRequest(new { Message = "Invalid user data. Please provide both email and password." });
        }

        // Check if the email already exists
        if (await _context.Users.AnyAsync(u => u.Email == user.Email))
        {
            return BadRequest(new { Message = "Email already exists. Please use a different email." });
        }

        // Begin a database transaction
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                // Add the new user to the database
                _context.Users.Add(user);
                await _context.SaveChangesAsync(); // Save user first to generate Id

                // Add user location
                _context.UserLocations.Add(new UserLocation
                {
                    UserId = user.Id,
                    Latitude = 0.0,
                    Longitude = 0.0,
                    IsSharingLocation = false
                });
                await _context.SaveChangesAsync();

                // Commit transaction if everything succeeds
                await transaction.CommitAsync();

                return Ok(new { Message = "User registered successfully!", User = user });
            }
            catch (Exception ex)
            {
                // Rollback the transaction on any error
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "An error occurred during registration.", Error = ex.Message });
            }
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromQuery] string email, [FromQuery] string password)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return BadRequest(new { Message = "Invalid credentials. Please provide both email and password." });
        }

        // Find the user by email and password
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

        // If user is not found, return unauthorized
        if (user == null)
        {
            return Unauthorized(new { Message = "Invalid email or password. Please try again." });
        }

        // Return the user details on successful login
        return Ok(new { Message = "Login successful!", User = user });
    }
    
    [HttpGet("users")]
    public async Task<IActionResult> Users()
    {
        var users = await _context.Users
            .Select(u => new 
            { 
                u.Id, 
                u.Name, 
                u.Email,
                u.Password
            })
            .ToListAsync();

        return Ok(new { Message = "Get all users", Users = users });
    }
    
    [HttpGet("user")]
    public async Task<IActionResult> User(int id)
    {
        var user = await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new 
            { 
                u.Id, 
                u.Name, 
                u.Email 
            })
            .FirstOrDefaultAsync(); // Fetch a single user

        if (user == null)
        {
            return NotFound(new { Message = "User not found" });
        }

        return Ok(new { Message = "User details", User = user });
    }


}