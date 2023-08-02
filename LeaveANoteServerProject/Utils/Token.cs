using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LeaveANoteServerProject.Utils
{
    public static class Token
    {
        private static int AdminId = 8;
        public static string CreateToken(User user, IConfiguration _configuration)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("userId", user.Id.ToString()),
               new Claim("deviceToken", user.DeviceToken),
               new Claim(ClaimTypes.Role, user.Role),
           };
           var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Secret").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
