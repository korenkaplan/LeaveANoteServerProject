using LeaveANoteServerProject.Data;
using LeaveANoteServerProject.Dto_s.User_Dto_s;
using LeaveANoteServerProject.DTO_s.User_Dto_s;
using LeaveANoteServerProject.Utils;
using Microsoft.EntityFrameworkCore;


namespace LeaveANoteServerProject.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public UserService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<HttpResponse<object>> RegisterUser(User user)
        {
            try
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                string token = Token.CreateToken(user, _configuration);
                return new HttpResponse<object> { IsSuccessful = true, Message = "Registration Succeed", Data = new { token }, StatusCode = 201 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<object> { IsSuccessful = false,Message="Registration Failed", Error = ex.Message, StatusCode = 500 };
            }
        }

        public async Task<HttpResponse<List<User>>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return new HttpResponse<List<User>> { IsSuccessful = true, Message = "All Database Users", Data = users, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<List<User>> { IsSuccessful = false,Message="Failed To Fetch All Users", Error = ex.Message ,StatusCode=500 };
            }
        }

        public async Task<HttpResponse<object>> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                if (user == null)
                {
                    return new HttpResponse<object> { IsSuccessful = false, Message = "Bad Credentials", StatusCode = 404 };
                }
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                {
                    return new HttpResponse<object> { IsSuccessful = false, Message = "Bad Credentials", StatusCode = 404 };

                }
                string token = Token.CreateToken(user, _configuration);
                return new HttpResponse<object> { IsSuccessful = true, Message = "Identifaction succeeded", Data = new { token }, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<object> { IsSuccessful = false, Message = "Failed To Fetch All Users", Error = ex.Message, StatusCode = 500 };
            }
        }

        public async Task<HttpResponse<string>> UpdateDeviceToken(DeviceTokenUpdateDto deviceTokenUpdateDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(deviceTokenUpdateDto.Id);
                if (user == null)
                {
                    return new HttpResponse<string> { IsSuccessful = false, Message = "Token Update Failed", Error = "The Id was not found in the database", StatusCode = 404 };
                }
                user.DeviceToken = deviceTokenUpdateDto.DeviceToken;
                await _context.SaveChangesAsync();
                return new HttpResponse<string> { IsSuccessful = true, Message = "Token updated successfully" };
            }
            catch (Exception ex)
            {
                return new HttpResponse<string> { IsSuccessful = false, Message = "Failed to update token", Error = ex.Message, StatusCode = 500 };
            }
        }

        public async Task<HttpResponse<string>> UpdateUserInformation(UpdateInformationDto updateInformationDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(updateInformationDto.Id);
                if (user == null)
                {
                    return new HttpResponse<string> { IsSuccessful = false, Message = "Information Update Failed", Error = "The Id was not found in the database", StatusCode = 404 };
                }
                user.UpdateInformation(updateInformationDto);
                await _context.SaveChangesAsync();
                return new HttpResponse<string> { IsSuccessful = true, Message = "Information updated successfully", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<string> { IsSuccessful = false, Message = "Failed to update information", Error = ex.Message, StatusCode = 500 };
            }
        }

        public async Task<HttpResponse<string>> UpdateUserPassword(UpdateUserPasswordDto updateUserPasswordDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(updateUserPasswordDto.Id);
                if (user == null)
                {
                    return new HttpResponse<string> { IsSuccessful = false, Message = "Password Update Failed", Error = "The Id was not found in the database", StatusCode = 404 };
                }
                user.UpdatePassword(updateUserPasswordDto);
                await _context.SaveChangesAsync();
                return new HttpResponse<string> { IsSuccessful = true, Message = "Password updated successfully", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<string> { IsSuccessful = false, Message = "Failed to update password", Error = ex.Message, StatusCode = 500 };
            }
        }

        public async Task<HttpResponse<GetUserByCarNumberDto>> GetUserByCarNumber(string carNumber)
        {
            try
            {
                var user = await _context.Users.Where(user => user.CarNumber == carNumber).FirstOrDefaultAsync();
                if (user == null)
                {
                    return new HttpResponse<GetUserByCarNumberDto> { IsSuccessful = false, Message = "Failed to fetch the user", Error = "The Id was not found in the database", StatusCode = 404 };
                }
                GetUserByCarNumberDto getUserByCarNumberDto = new GetUserByCarNumberDto(user.Id, user.CarNumber);

                return new HttpResponse<GetUserByCarNumberDto> { IsSuccessful = true, Message = "User found successfully", Data = getUserByCarNumberDto, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<GetUserByCarNumberDto> { IsSuccessful = false, Message = "Failed to fetch user", Error = ex.Message, StatusCode = 500 };
            }

        }

        public async Task<HttpResponse<User>> GetUserById(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return new HttpResponse<User> { IsSuccessful = false, Message = "Failed to fetch the user", Error = "The Id was not found in the database", StatusCode = 404 };
                }
                return new HttpResponse<User> { IsSuccessful = true, Message = "User found successfully", Data = user, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<User> { IsSuccessful = false, Message = "Failed to fetch user", Error = ex.Message, StatusCode = 500 };
            }
       
        }

        public async Task<HttpResponse<MinimalUserDto>> GetUserByIdMinimal(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return new HttpResponse<MinimalUserDto> { IsSuccessful = false, Message = "Failed to fetch the user", Error = "The Id was not found in the database", StatusCode = 404 };
                }
                MinimalUserDto minimalUserDto = new MinimalUserDto(user);
                return new HttpResponse<MinimalUserDto> { IsSuccessful = true, Message = "User found successfully", Data = minimalUserDto, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<MinimalUserDto> { IsSuccessful = false, Message = "Failed to fetch user", Error = ex.Message, StatusCode = 500 };
            }
        
        }
    }

}
