using gis_backend.ChatHub;
using gis_backend.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

builder.Services.AddSignalR();
app.MapHub<ChatHub>("/chatHub");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP to HTTPS (only in non-development environments)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Logging middleware for debugging
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request Method: {context.Request.Method}, Path: {context.Request.Path}");
    await next();
});

app.UseCors("AllowAll"); // Apply CORS policy
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request Method: {context.Request.Method}, Path: {context.Request.Path}");
    await next();
});
// app.UseAuthorization(); // Comment out if not needed
app.MapControllers();

app.Run();