using Microsoft.EntityFrameworkCore;

namespace LeaveANoteServerProject.Models
{

    public class MinimalUserDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string CarNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string DeviceToken { get; set; }

        public MinimalUserDto(User user)
        {
            Id = user.Id;
            Name = user.Name;
            CarNumber = user.CarNumber;
            PhoneNumber = user.PhoneNumber;
            DeviceToken = user.DeviceToken;
        }

    }
}
