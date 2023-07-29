namespace LeaveANoteServerProject.Models
{
    public class UnmatchedReport
    {
        public int Id { get; set; }
        public string DamagedCarNumber { get; set; }
        public int AccidentId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
