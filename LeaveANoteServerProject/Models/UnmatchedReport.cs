namespace LeaveANoteServerProject.Models
{
    public class UnmatchedReport
    {
        public int Id { get; set; }
        public string DamagedCarNumber { get; set; }
        public Accident Accident { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
