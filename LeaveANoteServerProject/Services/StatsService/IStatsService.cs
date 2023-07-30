using LeaveANoteServerProject.DTO_s.StatsDto_s;

namespace LeaveANoteServerProject.Services.StatsService
{
    public interface IStatsService
    {
        Task<HttpResponse<RegisteredUsersDto>> RegisteredUsersData(RegisteredUserDataDtoReq registeredUserDataDtoReq);
        Task<HttpResponse<ReportsDistributionDto>> ReportsDistributtion(ReportsDistributtionDtoReq reportsDistributtionDtoReq);
    }
}
