using LeaveANoteServerProject.DTO_s.Accident_Dto_s;

namespace LeaveANoteServerProject.DTO_s.StatsDto_s
{
    /// <summary>
    /// Represents the request data for retrieving registered users' data.
    /// </summary>
    public class RegisteredUsersDto
    {
        public List<MonthlyUsersDto> MonthlyUsers { get; set; } = new List<MonthlyUsersDto>();
    }
}
