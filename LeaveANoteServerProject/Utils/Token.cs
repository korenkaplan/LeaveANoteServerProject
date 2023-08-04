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
        public static string JWTKEY {get; set;}
        public static string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
               new Claim("deviceToken", user.DeviceToken),
               new Claim(ClaimTypes.Role, user.Role),
           };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTKEY));

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
