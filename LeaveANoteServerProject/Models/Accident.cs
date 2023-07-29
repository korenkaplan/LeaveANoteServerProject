using LeaveANoteServerProject.Interfaces;

namespace LeaveANoteServerProject.Models
{
    public class Accident
    {
        public int Id { get; set; }
        public IHittingDriver HittingDriver { get; set; }
        public IReporter Reporter { get; set; }
        public string ImageSource { get; set; }
        public string Type { get; set; }
        public bool IsAnonymous { get; set; }
        public bool IsIdentify { get; set; }
        public bool IsDeleted { get; set; }
        public string Date { get; set; }
        public DateTime createdAt { get; set; }
    }
}
