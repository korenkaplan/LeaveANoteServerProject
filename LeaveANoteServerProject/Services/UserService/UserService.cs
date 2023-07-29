using BCrypt.Net;
using LeaveANoteServerProject.Data;
using LeaveANoteServerProject.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LeaveANoteServerProject.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public UserService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<HttpResponse<object>> RegisterUser(User user)
        {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                string token = Token.CreateToken(user, _configuration);
                return new HttpResponse<object> { IsSuccessful = true, Message = "saved successfully",Data = new { token } };
        }

        public async Task<HttpResponse<List<User>>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return new HttpResponse<List<User>>{ IsSuccessful = true, Message = "All Database Users", Data=users };
            }
            catch (Exception ex)
            {
                return new HttpResponse<List<User>> { IsSuccessful = false, Message = ex.Message };
            }
        }

        public async Task<HttpResponse<object>> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return new HttpResponse<object> { IsSuccessful = false, Message = " email  inccorect" };
                }
                if(!BCrypt.Net.BCrypt.Verify(password,user.Password))
                {
                    return new HttpResponse<object> { IsSuccessful = false, Message = "Password  inccorect" };
                }
                string token = Token.CreateToken(user, _configuration);
            return new HttpResponse<object> { IsSuccessful=true, Message ="Identifaction succeeded", Data = new { token } };
        }

    }
}
