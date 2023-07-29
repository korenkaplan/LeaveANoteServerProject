using LeaveANoteServerProject.Dto_s.User_Dto_s;
using LeaveANoteServerProject.Models;
using LeaveANoteServerProject.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeaveANoteServerProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterNewUser([FromBody] User user)
        {
            HttpResponse<object> res = await _userService.RegisterUser(user);
            return Ok(res);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            HttpResponse<object> res = await _userService.Login(loginDto.Email, loginDto.Password);
            return Ok(res);
        }

        [HttpGet("allUsers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            HttpResponse<List<User>> res = await _userService.GetAllUsers();
            return Ok(res);
        }
    }
}
