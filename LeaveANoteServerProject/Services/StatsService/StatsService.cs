using LeaveANoteServerProject.DTO_s.StatsDto_s;

namespace LeaveANoteServerProject.Services.StatsService
{
    public class StatsService : IStatsService
    {
        public Task<HttpResponse<RegisteredUsersDto>> RegisteredUsersData(RegisteredUserDataDtoReq registeredUserDataDtoReq)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponse<ReportsDistributionDto>> ReportsDistributtion(ReportsDistributtionDtoReq reportsDistributtionDtoReq)
        {
            throw new NotImplementedException();
        }
    }
}
