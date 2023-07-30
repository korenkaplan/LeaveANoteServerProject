using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveANoteServerProject.Models
{
    public class Accident
    {
        public int Id { get; set; }
        public string HittingDriverName { get; set; } = string.Empty;
        public string HittingCarNumber { get; set; } = string.Empty;
        public string HittingDriverPhoneNumber { get; set; } = string.Empty;
        public string ReporterName { get; set; } = string.Empty;
        public string ReporterPhoneNumber { get; set; } = string.Empty;
        public string ImageSource { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsAnonymous { get; set; }
        public bool IsIdentify { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsRead { get; set; } 
        public string Date { get; set; } = DateTime.Now.ToString("dd-MM-yyyy");

        public DateTime createdAt { get; set; } = DateTime.Now;

    }
}
