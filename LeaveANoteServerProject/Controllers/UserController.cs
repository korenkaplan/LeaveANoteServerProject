using LeaveANoteServerProject.Dto_s.User_Dto_s;
using LeaveANoteServerProject.DTO_s.User_Dto_s;
using LeaveANoteServerProject.Models;
using LeaveANoteServerProject.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Linq.Expressions;

namespace LeaveANoteServerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] 
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register"),AllowAnonymous]
        public async Task<IActionResult> RegisterNewUser([FromBody] User user)
        {
            HttpResponse<object> res = await _userService.RegisterUser(user);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("login"), AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            HttpResponse<object> res = await _userService.Login(loginDto);
            Log.Information("Respond=> {@res}", res);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("allUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            HttpResponse<List<User>> res = await _userService.GetAllUsers();
            return StatusCode(res.StatusCode, res); ;
        }

        [HttpPost("updateDeviceToken")]
        public async Task<IActionResult> UpdateDeviceToken(DeviceTokenUpdateDto deviceTokenUpdateDto )
        {
            HttpResponse<string> res = await _userService.UpdateDeviceToken(deviceTokenUpdateDto);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("informationUpdate")]
        public async Task<IActionResult> UpdateUserInformation(UpdateInformationDto updateInformationDto)
        {
            HttpResponse<string> res = await _userService.UpdateUserInformation(updateInformationDto);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPost("passwordUpdate")]
        public async Task<IActionResult> UpdateUserPassword(UpdateUserPasswordDto updateUserPasswordDto)
        {
            HttpResponse<string> res = await _userService.UpdateUserPassword(updateUserPasswordDto);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("searchCarNumber/{carNumber}")]
        public async Task<IActionResult> GetUserByCarNumber(string carNumber)
        {
            HttpResponse<GetUserByCarNumberDto> res = await _userService.GetUserByCarNumber(carNumber);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            HttpResponse<User> res = await _userService.GetUserById(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("getByIdMinimal/{id}")]
        public async Task<IActionResult> GetMinimalUserById(int id)
        {
            HttpResponse<MinimalUserDto> res = await _userService.GetMinimalUserById(id);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPost("deleteMessage")]
        public async Task<IActionResult> DeleteAccidentFromInbox(AccidentDeleteDto accidentDeleteDto)
        {
            HttpResponse<string> res = await _userService.DeleteAccidentFromInbox(accidentDeleteDto);
            return StatusCode(res.StatusCode, res);
        }
        [HttpPost("readMessageInbox")]
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
