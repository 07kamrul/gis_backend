using gis_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace gis_backend.Repositories;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    
    public DbSet<UserLocation> UserLocations { get; set; } = null!;
}