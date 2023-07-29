using LeaveANoteServerProject.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveANoteServerProject.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<UnmatchedReport> UnmatchedReports { get; set; }
        public DbSet<Accident> Accidents { get; set; }

    }
}
