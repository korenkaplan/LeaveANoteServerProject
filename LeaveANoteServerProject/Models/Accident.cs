using LeaveANoteServerProject.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveANoteServerProject.Models
{
    public class Accident
    {
        public int Id { get; set; }
        public string HittingDriverName { get; set; }
        public string HittingCarNumber { get; set; }
        public string HittingDriverPhoneNumber { get; set; }
        public string ReporterName { get; set; }
        public string ReporterPhoneNumber { get; set; }
        public string ImageSource { get; set; }
        public string Type { get; set; }
        public bool IsAnonymous { get; set; }
        public bool IsIdentify { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsRead { get; set; }
        public string Date { get; set; }
        public DateTime createdAt { get; set; }
    }
}
