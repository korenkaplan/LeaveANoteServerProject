using LeaveANoteServerProject.DTO_s.Accident_Dto_s;
using LeaveANoteServerProject.DTO_s.StatsDto_s;
using LeaveANoteServerProject.Utils;

namespace LeaveANoteServerProject.Services.StatsService
{
    public interface IStatsService
    {
        /// <summary>
        /// Retrieves data for registered users based on the specified year.
        /// </summary>
        /// <param name="registeredUserDataDtoReq">The data transfer object containing the year and the user's role.</param>
        /// <returns>An HTTP response containing the registered users' data.</returns>
        Task<HttpResponse<List<MonthlyUsersDto>>> RegisteredUsersData(int year);

        /// <summary>
        /// Retrieves the distribution of reports, notes and unmatched reports.
        /// </summary>
        /// <param name="reportsDistributtionDtoReq">The data transfer object containing the user's role.</param>
        /// <returns>An HTTP response containing the distribution of reports.</returns>
        Task<HttpResponse<List<ReportDistributionItemDto>>> ReportsDistributtion();
    }
}
