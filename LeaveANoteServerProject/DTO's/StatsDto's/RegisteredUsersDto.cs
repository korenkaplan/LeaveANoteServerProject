using LeaveANoteServerProject.DTO_s.Accident_Dto_s;

namespace LeaveANoteServerProject.DTO_s.StatsDto_s
{
    public class RegisteredUsersDto
    {
        public List<MonthlyUsersDto> MonthlyUsers { get; set; } = new List<MonthlyUsersDto>();
    }
}
