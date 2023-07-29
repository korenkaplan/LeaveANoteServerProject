using LeaveANoteServerProject.DTO_s.User_Dto_s;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveANoteServerProject.Models
{

    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(CarNumber), IsUnique = true)]
    [Index(nameof(PhoneNumber), IsUnique = true)]
    [Index(nameof(DeviceToken), IsUnique = true)]
    public class User
    { 
        public int Id { get; set; }
        public string Name { get; set; }
        public string CarNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string DeviceToken { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "user";
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<Accident> Accidents { get; set; } = new List<Accident>();

        public void UpdateInformation(UpdateInformationDto updateInformationDto)
        {
            Name = updateInformationDto.Name;
            Email = updateInformationDto.Email;
            CarNumber = updateInformationDto.CarNumber;
            PhoneNumber = updateInformationDto.PhoneNumber;
        }

        public bool UpdatePassword(UpdateUserPasswordDto updatePasswordDto)
        {
            if (!BCrypt.Net.BCrypt.Verify(updatePasswordDto.OldPassword, Password))
            {
                return false;
            }
            Password = BCrypt.Net.BCrypt.HashPassword(updatePasswordDto.NewPassword);
            return true;
        }
    }
}
