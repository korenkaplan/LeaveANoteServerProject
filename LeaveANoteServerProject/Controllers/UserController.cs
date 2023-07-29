using LeaveANoteServerProject.Dto_s.User_Dto_s;
using LeaveANoteServerProject.DTO_s.User_Dto_s;
using LeaveANoteServerProject.Models;
using LeaveANoteServerProject.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

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
        public async Task<IActionResult> RegisterNewUser([FromBody] User user)
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

        [HttpGet("allUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            HttpResponse<List<User>> res = await _userService.GetAllUsers();
            return StatusCode(res.StatusCode, res); ;
        }

    }
}
