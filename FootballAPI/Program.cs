
using Microsoft.EntityFrameworkCore;
using FootballAPI.Data;
using FootballAPI.Repository;
using FootballAPI.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context - Using In-Memory Database for development
builder.Services.AddDbContext<FootballDbContext>(options =>
    options.UseInMemoryDatabase("FootballDB"));

// Repository Registration
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();

// Service Registration
builder.Services.AddScoped<IPlayerService, PlayerService>();

// CORS Configuration for Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular dev server
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();

