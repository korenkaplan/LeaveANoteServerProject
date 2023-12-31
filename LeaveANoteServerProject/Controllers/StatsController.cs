﻿using LeaveANoteServerProject.DTO_s.Accident_Dto_s;
using LeaveANoteServerProject.DTO_s.StatsDto_s;
using LeaveANoteServerProject.Services.StatsService;
using LeaveANoteServerProject.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeaveANoteServerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class StatsController : ControllerBase
    {
        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet("registeredUsersData")]
        public async Task<IActionResult> GetRegisteredUsersData([FromQuery]int year)
        {
            HttpResponse<List<MonthlyUsersDto>> res = await _statsService.RegisteredUsersData(year);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("reportsDistribution/")]
        public async Task<IActionResult> GetReportsDistributionData()
        {
            HttpResponse<List<ReportDistributionItemDto>> res = await _statsService.ReportsDistributtion();
            return StatusCode(res.StatusCode, res);
        }
    }
}
