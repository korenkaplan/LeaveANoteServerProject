global using LeaveANoteServerProject.Models;
global using LeaveANoteServerProject.Dto_s;
using LeaveANoteServerProject.Data;
using Microsoft.EntityFrameworkCore;
using LeaveANoteServerProject.Services.UserService;
using Serilog;
using LeaveANoteServerProject.Services.AccidentService;
using LeaveANoteServerProject.Services.StatsService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//add the configuration for serilog logger
Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();

//Add Enteties services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccidentService, AccidentService>();
builder.Services.AddScoped<IStatsService, StatsService>();

//add JWT Authentication
builder.Services.AddAuthentication().AddJwtBearer();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
