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
    public async Task<IActionResult> Register(User user)
    {
        // Check if the email already exists
        if (await _context.Users.AnyAsync(u => u.Email == user.Email))
        {
            return BadRequest(new { Message = "Email already exists. Please use a different email." });
        }

        // Add the new user to the database
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Return success message
        return Ok(new { Message = "User registered successfully!", User = user });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string email, string password)
    {
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
}