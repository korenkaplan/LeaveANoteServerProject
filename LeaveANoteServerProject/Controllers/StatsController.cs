using LeaveANoteServerProject.DTO_s.StatsDto_s;
using LeaveANoteServerProject.Services.StatsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeaveANoteServerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet("registeredUsersData")]
        public async Task<IActionResult> GetRegisteredUsersData(RegisteredUserDataDtoReq registeredUserDataDtoReq)
        {
            HttpResponse<RegisteredUsersDto> res = await _statsService.RegisteredUsersData(registeredUserDataDtoReq);
            return StatusCode(res.StatusCode, res);
        }
        [HttpGet("reportsDistribution")]
        public async Task<IActionResult> GetRegisteredUsersData(ReportsDistributtionDtoReq reportsDistributtionDtoReq)
        {
            HttpResponse<ReportsDistributionDto> res = await _statsService.ReportsDistributtion(reportsDistributtionDtoReq);
            return StatusCode(res.StatusCode, res);
        }
    }
}
