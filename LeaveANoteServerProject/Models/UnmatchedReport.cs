using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace LeaveANoteServerProject.Models
{
    public class UnmatchedReport
    {
        public int Id { get; set; }
        public string DamagedCarNumber { get; set; } = string.Empty;
        public Accident Accident { get; set; }

        public bool IsUnmatched { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
