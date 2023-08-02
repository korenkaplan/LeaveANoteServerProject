using LeaveANoteServerProject.Dto_s.User_Dto_s;
using LeaveANoteServerProject.DTO_s.User_Dto_s;
using LeaveANoteServerProject.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace LeaveANoteServerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register"),AllowAnonymous]
        public async Task<IActionResult> RegisterNewUser(User user)
        {
            HttpResponse<object> res = await _userService.RegisterUser(user);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            HttpResponse<object> res = await _userService.Login(loginDto);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("allUsers"), AllowAnonymous]
        public async Task<IActionResult> GetAllUsers()
        {
            HttpResponse<List<User>> res = await _userService.GetAllUsers();
            return StatusCode(res.StatusCode, res); ;
        }

        [HttpPut("updateDeviceToken")]
        public async Task<IActionResult> UpdateDeviceToken(DeviceTokenUpdateDto deviceTokenUpdateDto )
        {
            HttpResponse<string> res = await _userService.UpdateDeviceToken(deviceTokenUpdateDto);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("informationUpdate")]
        public async Task<IActionResult> UpdateUserInformation(UpdateInformationDto updateInformationDto)
        {
            HttpResponse<User> res = await _userService.UpdateUserInformation(updateInformationDto);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("passwordUpdate")]
        public async Task<IActionResult> UpdateUserPassword(UpdateUserPasswordDto updateUserPasswordDto)
        {
            HttpResponse<string> res = await _userService.UpdateUserPassword(updateUserPasswordDto);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("searchCarNumber/")]
        public async Task<IActionResult> GetUserByCarNumber([FromQuery] string carNumber)
        {
            HttpResponse<GetUserByCarNumberDto> res = await _userService.GetUserByCarNumber(carNumber);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("getById/")]
        public async Task<IActionResult> GetUserById([FromQuery] int id)
        {
            HttpResponse<User> res = await _userService.GetUserById(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("getByIdMinimal/")]
        public async Task<IActionResult> GetMinimalUserById([FromQuery] int id)
        {
            HttpResponse<MinimalUserDto> res = await _userService.GetMinimalUserById(id);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPut("readMessageInbox")]
        public async Task<IActionResult> DeleteAccidentFromInbox(AccidentDeleteDto accidentDeleteDto)
        {
            HttpResponse<string> res = await _userService.DeleteAccidentFromInbox(accidentDeleteDto);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPost("deleteMessage")]
        public async Task<IActionResult> DeleteAccidentFromHistory(AccidentDeleteDto accidentDeleteDto)
        {
            HttpResponse<string> res = await _userService.DeleteAccidentFromHistory(accidentDeleteDto);
            return StatusCode(res.StatusCode, res);
        }

        [HttpDelete("deleteUser/{id}")]
        public async Task<IActionResult> DeleteUserByID(int id)
        {
            HttpResponse<string> res = await _userService.DeleteUserById(id);
            return StatusCode(res.StatusCode, res);
        }
    }
}
