using LeaveANoteServerProject.DTO_s.Accident_Dto_s;
using LeaveANoteServerProject.Services.AccidentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeaveANoteServerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccidentController : ControllerBase
    {

        private readonly IAccidentService _accidentService;

        public AccidentController(IAccidentService accidentService)
        {
            _accidentService = accidentService;
        }

        [HttpPost("createNote")]
        public async Task<IActionResult> CreateNote(CreateNoteReqDto createNoteReqDto)
        {
            HttpResponse<Accident> res = await _accidentService.CreateNote(createNoteReqDto);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("createReport")]
        public async Task<IActionResult> CreateReport(CreateReportReqDto createReportReqDto)
        {
            HttpResponse<Accident> res = await _accidentService.CreateReport(createReportReqDto);
            return StatusCode(res.StatusCode, res);
        }
    }
}
