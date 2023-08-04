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
        In = ParameterLocation.Header   ,
        Name="Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
//add the configuration for serilog logger
Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();

if (builder.Environment.IsProduction())
{
    // get the configuration for the Azure key vault from app settings
    var keyVaultUrl = builder.Configuration.GetSection("KeyVault:KeyVaultUrl");
    var keyVaultClientId = builder.Configuration.GetSection("KeyVault:ClientId");
    var keyVaultClientSecret = builder.Configuration.GetSection("KeyVault:ClientSecret");
    var keyVaultDirectoryID = builder.Configuration.GetSection("KeyVault:DirectoryID");

    //creat the credentials and the client
    var credential = new ClientSecretCredential(keyVaultDirectoryID.Value!.ToString(), keyVaultClientId.Value!.ToString(), keyVaultClientSecret.Value!.ToString());
    builder.Configuration.AddAzureKeyVault(keyVaultUrl.Value!.ToString(), keyVaultClientId.Value!.ToString(), keyVaultClientSecret.Value!.ToString(), new DefaultKeyVaultSecretManager());
    var client = new SecretClient(new Uri(keyVaultUrl.Value!.ToString()), credential);

    //add the connections trign from the vault and assign in to the datacontext
    string CONSTRING = client.GetSecret("DefaultConnection").Value.Value.ToString();
    builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(CONSTRING));

    //get the JWT Secret key from the key vault and assign it to the the Token Class
    string JWTTOKEN = client.GetSecret("JWTTOKEN").Value.Value.ToString();
    Token.JWTKEY = JWTTOKEN;
}

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    string JWTTOKEN = builder.Configuration.GetSection("AppSettings:Secret").Value!;
    Token.JWTKEY = JWTTOKEN;

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
