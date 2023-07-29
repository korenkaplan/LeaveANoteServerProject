using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveANoteServerProject.Models
{
    [Index(nameof(Email),IsUnique = true)]
    [Index(nameof(CarNumber),IsUnique = true)]
    [Index(nameof(PhoneNumber),IsUnique = true)]
    [Index(nameof(DeviceToken),IsUnique = true)]

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CarNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; } = "user";
        public string DeviceToken { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<Accident> Accidents { get; set; } = new List<Accident>();
    }
}
