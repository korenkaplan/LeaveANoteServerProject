global using LeaveANoteServerProject.Models;
global using LeaveANoteServerProject.Dto_s;
using LeaveANoteServerProject.Data;
using Microsoft.EntityFrameworkCore;
using LeaveANoteServerProject.Services.UserService;
using Serilog;
using LeaveANoteServerProject.Services.AccidentService;
using LeaveANoteServerProject.Services.StatsService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;
using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Azure.Security.KeyVault.Secrets;
using Newtonsoft.Json.Linq;
using LeaveANoteServerProject.Utils;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

//add the configuration for serilog logger
Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();

if (builder.Environment.IsProduction())
{
    //get the azure key vault url from the Enviorment variables of th App Service 
    var keyVaultUrl = Environment.GetEnvironmentVariable("keyVaultUrl");
    if (string.IsNullOrEmpty(keyVaultUrl))
    {
        throw new InvalidOperationException("Key Vault URL not configured.");
    }
    var client = new SecretClient(new Uri(keyVaultUrl), new ManagedIdentityCredential());

    //get the connection string and the JWT key 
    KeyVaultSecret CONSTRING = client.GetSecret("DefaultConnection");
    KeyVaultSecret JWTKEY = client.GetSecret("JWTKEY");
    string conString = CONSTRING.Value;
    string jwtKey = JWTKEY.Value;
    Token.JWTKEY = jwtKey;
    builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(conString));

}

if (builder.Environment.IsDevelopment())
{
    string conString = builder.Configuration["ConnectionsStrings:defaultConnection"];
    string jwtToken = builder.Configuration["JWTKEY"];
    builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(conString));
    Token.JWTKEY = jwtToken;
}

//add JWT Authentication
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Token.JWTKEY))
    };
}
);

//Add Enteties services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccidentService, AccidentService>();
builder.Services.AddScoped<IStatsService, StatsService>();


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
