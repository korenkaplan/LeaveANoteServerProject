using LeaveANoteServerProject.Data;
using LeaveANoteServerProject.Dto_s.User_Dto_s;
using LeaveANoteServerProject.DTO_s.User_Dto_s;
using LeaveANoteServerProject.Utils;
using Microsoft.Data.SqlClient;
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

        #region User Service Functions
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
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlException && IsDuplicateKeyError(sqlException))
                {
                    string errorMessage = GetDuplicateKeyErrorMessage(sqlException);
                    return new HttpResponse<object> { IsSuccessful = false, Message = "Failed to update information", Error = errorMessage, StatusCode = 400 };
                }

                return new HttpResponse<object> { IsSuccessful = false, Message = "Failed to update information", Error = ex.Message, StatusCode = 500 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<object> { IsSuccessful = false, Message = "Registration Failed", Error = ex.Message, StatusCode = 500 };
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
                return new HttpResponse<List<User>> { IsSuccessful = false, Message = "Failed To Fetch All Users", Error = ex.Message, StatusCode = 500 };
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

        public async Task<HttpResponse<string>> UpdateUserPassword(UpdateUserPasswordDto updateUserPasswordDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(updateUserPasswordDto.Id);
                if (user == null)
                {
                    return new HttpResponse<string> { IsSuccessful = false, Message = "Password Update Failed", Error = "The Id was not found in the database", StatusCode = 404 };
                }
                bool isUpdated = user.UpdatePassword(updateUserPasswordDto);
                if (!isUpdated) { return new HttpResponse<string> { IsSuccessful = false, Message = "Password Update Failed", Error = "old Password is incorrect", StatusCode = 400 }; }
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
                GetUserByCarNumberDto getUserByCarNumberDto = new GetUserByCarNumberDto(user.Id, user.DeviceToken);

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

        public async Task<HttpResponse<MinimalUserDto>> GetMinimalUserById(int id)
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

        public async Task<HttpResponse<string>> DeleteAccidentFromInbox(AccidentDeleteDto accidentDeleteDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(accidentDeleteDto.UserId);
                if (user == null)
                {
                    return new HttpResponse<string> { IsSuccessful = false, Message = "Failed to fetch the user", Error = "The Id was not found in the database", StatusCode = 404 };
                }

                Accident accident = user.Accidents.FirstOrDefault(a => a.Id == accidentDeleteDto.AccidentId);
                if (accident == null)
                {
                    return new HttpResponse<string> { IsSuccessful = false, Message = "Failed to fetch the accident", Error = "The Id was not found in the user's accidents list", StatusCode = 404 };
                }
                accident.IsRead = true;
                await _context.SaveChangesAsync();

                return new HttpResponse<string> { IsSuccessful = true, Message = "Message has been marked as read", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<string> { IsSuccessful = false, Message = "Failed to Mark message as read", Error = ex.Message, StatusCode = 500 };
            }
        }

        public async Task<HttpResponse<string>> DeleteAccidentFromHistory(AccidentDeleteDto accidentDeleteDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(accidentDeleteDto.UserId);
                if (user == null)
                {
                    return new HttpResponse<string> { IsSuccessful = false, Message = "Failed to fetch the user", Error = "The Id was not found in the database", StatusCode = 404 };
                }

                Accident accident = user.Accidents.FirstOrDefault(a => a.Id == accidentDeleteDto.AccidentId);
                if (accident == null)
                {
                    return new HttpResponse<string> { IsSuccessful = false, Message = "Failed to fetch the accident", Error = "The Id was not found in the user's accidents list", StatusCode = 404 };
                }
                accident.IsDeleted = true;
                await _context.SaveChangesAsync();

                return new HttpResponse<string> { IsSuccessful = true, Message = "Message has been marked as Deleted", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<string> { IsSuccessful = false, Message = "Failed to Mark message as deleted", Error = ex.Message, StatusCode = 500 };
            }
        }

        public async Task<HttpResponse<string>> DeleteUserById(int id)
        {
            try
            {
                User user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    return new HttpResponse<string> { IsSuccessful = true, Message = $"User with the id: {id} was removed", StatusCode = 200 };
                }
                return new HttpResponse<string> { IsSuccessful = false, Message = $"User with the id: {id} was't found", StatusCode = 404 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<string> { IsSuccessful = false, Message = "Failed to deleted user", Error = ex.Message, StatusCode = 500 };
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
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlException && IsDuplicateKeyError(sqlException))
                {
                    string errorMessage = GetDuplicateKeyErrorMessage(sqlException);
                    return new HttpResponse<string> { IsSuccessful = false, Message = "Failed to update information", Error = errorMessage, StatusCode = 400 };
                }

                return new HttpResponse<string> { IsSuccessful = false, Message = "Failed to update information", Error = ex.Message, StatusCode = 500 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<string> { IsSuccessful = false, Message = "Failed to update information", Error = ex.Message, StatusCode = 500 };
            }
        } 
        #endregion

        #region Index Duplication Error Functions 
        /// <summary>
        /// Check if the given exception indicates a duplicate key (unique index violation) error in SQL Server.
        /// </summary>
        /// <param name="ex">The SQL exception to check.</param>
        /// <returns>True if the exception indicates a duplicate key error; otherwise, false.</returns>
        private bool IsDuplicateKeyError(SqlException ex)
        {
            // SQL Server error code for unique index violation is 2601
            // You can also check for error number 2627, which corresponds to the same issue.
            return ex.Number == 2601 || ex.Number == 2627;
        }

        /// <summary>
        /// Get the error message for a duplicate key (unique index violation) error.
        /// And return it in a user readable form.
        /// </summary>
        /// <param name="ex">The SQL exception containing the error details.</param>
        /// <returns>The error message indicating which field caused the duplicate key issue.</returns>
        private string GetDuplicateKeyErrorMessage(SqlException ex)
        {
            // Extract the index name from the exception message to indicate which field caused the issue.

            int startIndex = ex.Message.IndexOf('\'');
            int endIndex = ex.Message.LastIndexOf('\'');
            string indexName = ex.Message.Substring(startIndex + 1, endIndex - startIndex - 1);

            // You can create a mapping to convert index names to more user-friendly field names if needed.
            string field;
            if (indexName.Contains("CarNumber"))
                field = "car number";
            else if (indexName.Contains("PhoneNumber"))
                field = "phone number";
            else if (indexName.Contains("DeviceToken"))
                field = "device token";
            else if (indexName.Contains("Email"))
                field = "email";
            else
                field = "field";

            return $"This {field} is aleady in use by another user";
        } 
        #endregion

    }

}
