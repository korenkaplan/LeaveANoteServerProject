﻿using LeaveANoteServerProject.Data;
using LeaveANoteServerProject.Dto_s.User_Dto_s;
using LeaveANoteServerProject.DTO_s.User_Dto_s;
using LeaveANoteServerProject.Models;
using LeaveANoteServerProject.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
                await CheckForMatchedReports(user);
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                string token = Token.CreateToken(user);
                return new HttpResponse<object> { Success = true, Message = "Registration Succeed", Data = token, StatusCode = 201 };
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlException && IsDuplicateKeyError(sqlException))
                {
                    string errorMessage = GetDuplicateKeyErrorMessage(sqlException);
                    return new HttpResponse<object> { Success = false, Message = "Registration Failed", Error = errorMessage, StatusCode = 400 };
                }

                return new HttpResponse<object> { Success = false, Message = "Registration Failed", Error = ex.InnerException.Message, StatusCode = 500 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<object> { Success = false, Message = "Registration Failed", Error = ex.InnerException.Message, StatusCode = 500 };
            }
        }

        private async Task CheckForMatchedReports(User user)
        {
            List<UnmatchedReport> newUserAccidents = await _context.UnmatchedReports
                .Where(report => report.DamagedCarNumber == user.CarNumber).Include(r => r.Accident).ToListAsync();

            // Explicitly loading the Accidents navigation property
            foreach (UnmatchedReport unmatchedReport in newUserAccidents)
            {
                user.Accidents.Add(unmatchedReport.Accident);
                unmatchedReport.IsUnmatched = true;
                int id = unmatchedReport.Accident.Id;
                Log.Information("added accident: {@id}", id);
            }
        }

        public async Task<HttpResponse<List<User>>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.Include(u => u.Accidents).ToListAsync();

                users.ForEach(user => {
                user.Accidents = user.Accidents.Where(accident => !accident.IsDeleted).ToList();
                });
                // Filter accidents where isDeleted == false
                return new HttpResponse<List<User>> { Success = true, Message = "All Database Users", Data = users, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<List<User>> { Success = false, Message = "Failed To Fetch All Users", Error = ex.InnerException.Message, StatusCode = 500 };
            }
        }

        public async Task<HttpResponse<object>> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                if (user == null)
                {
                    return new HttpResponse<object> { Success = false, Message = "Bad Credentials", StatusCode = 404 };
                }
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                {
                    return new HttpResponse<object> { Success = false, Message = "Bad Credentials", StatusCode = 404 };

                }
                string token = Token.CreateToken(user);
                return new HttpResponse<object> { Success = true, Message = "Identifaction succeeded", Data = new { token }, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<object> { Success = false, Message = "Failed To Fetch All Users", Error = ex.InnerException.Message, StatusCode = 500 };
            }
        }

        public async Task<HttpResponse<string>> UpdateDeviceToken(DeviceTokenUpdateDto deviceTokenUpdateDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(deviceTokenUpdateDto.Id);
                if (user == null)
                {
                    return new HttpResponse<string> { Success = false, Message = "Token Update Failed", Error = "The Id was not found in the database", StatusCode = 404 };
                }
                user.DeviceToken = deviceTokenUpdateDto.DeviceToken;
                await _context.SaveChangesAsync();
                return new HttpResponse<string> { Success = true, Message = "Token updated successfully", Data = user.DeviceToken, StatusCode = 204 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<string> { Success = false, Message = "Failed to update token", Error = ex.InnerException.Message, StatusCode = 500 };
            }
        }

        public async Task<HttpResponse<string>> UpdateUserPassword(UpdateUserPasswordDto updateUserPasswordDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(updateUserPasswordDto.Id);
                if (user == null)
                {
                    return new HttpResponse<string> { Success = false, Message = "Password Update Failed", Error = "The Id was not found in the database", StatusCode = 404 };
                }
                bool isUpdated = user.UpdatePassword(updateUserPasswordDto);
                if (!isUpdated) { return new HttpResponse<string> { Success = false, Message = "Password Update Failed", Error = "old Password is incorrect", StatusCode = 400 }; }
                await _context.SaveChangesAsync();
                return new HttpResponse<string> { Success = true, Message = "Password updated successfully", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<string> { Success = false, Message = "Failed to update password", Error = ex.InnerException.Message, StatusCode = 500 };
            }
        }

        public async Task<HttpResponse<GetUserByCarNumberDto>> GetUserByCarNumber(string carNumber)
        {
            try
            {
                var user = await _context.Users.Where(user => user.CarNumber == carNumber).FirstOrDefaultAsync();
                if (user == null)
                {
                    return new HttpResponse<GetUserByCarNumberDto> { Success = false, Message = "Failed to fetch the user", Error = "The Id was not found in the database", StatusCode = 404 };
                }
                GetUserByCarNumberDto getUserByCarNumberDto = new GetUserByCarNumberDto(user.Id, user.DeviceToken);

                return new HttpResponse<GetUserByCarNumberDto> { Success = true, Message = "User found successfully", Data = getUserByCarNumberDto, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<GetUserByCarNumberDto> { Success = false, Message = "Failed to fetch user", Error = ex.InnerException.Message, StatusCode = 500 };
            }

        }

        public async Task<HttpResponse<User>> GetUserById(int id, bool minimal)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return new HttpResponse<User> { Success = false, Message = "Failed to fetch the user", Error = "The Id was not found in the database", StatusCode = 404 };
                }
                if(!minimal)
                {
                    // Explicitly loading the Accidents navigation property
                    await _context.Entry(user).Collection(u => u.Accidents).LoadAsync();

                    // Filter accidents where isDeleted == false
                    user.Accidents = user.Accidents.Where(accident => !accident.IsDeleted).ToList();
                }
                user.Password = "";

                return new HttpResponse<User> { Success = true, Message = "User found successfully", Data = user, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<User> { Success = false, Message = "Failed to fetch user", Error = ex.InnerException.Message, StatusCode = 500 };
            }

        }


        public async Task<HttpResponse<string>> DeleteAccidentFromInbox(AccidentDeleteDto accidentDeleteDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(accidentDeleteDto.UserId);
                if (user == null)
                {
                    return new HttpResponse<string> { Success = false, Message = "Failed to fetch the user", Error = "The Id was not found in the database", StatusCode = 404 };
                }

                await _context.Entry(user).Collection(u => u.Accidents).LoadAsync();
                Accident accident = user.Accidents.FirstOrDefault(a => a.Id == accidentDeleteDto.AccidentId);
                if (accident == null)
                {
                    return new HttpResponse<string> { Success = false, Message = "Failed to fetch the accident", Error = "The Id was not found in the user's accidents list", StatusCode = 404 };
                }
                accident.IsRead = true;
                await _context.SaveChangesAsync();

                return new HttpResponse<string> { Success = true, Message = "Message has been marked as read", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<string> { Success = false, Message = "Failed to Mark message as read", Error = ex.InnerException.Message, StatusCode = 500 };
            }
        }

        public async Task<HttpResponse<string>> DeleteAccidentFromHistory(AccidentDeleteDto accidentDeleteDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(accidentDeleteDto.UserId);
                if (user == null)
                {
                    return new HttpResponse<string> { Success = false, Message = "Failed to fetch the user", Error = "The Id was not found in the database", StatusCode = 404 };
                }
                await _context.Entry(user).Collection(u => u.Accidents).LoadAsync();
                Accident accident = user.Accidents.FirstOrDefault(a => a.Id == accidentDeleteDto.AccidentId);
                if (accident == null)
                {
                    return new HttpResponse<string> { Success = false, Message = "Failed to fetch the accident", Error = "The Id was not found in the user's accidents list", StatusCode = 404 };
                }
                accident.IsDeleted = true;
                await _context.SaveChangesAsync();

                return new HttpResponse<string> { Success = true, Message = "Message has been deleted successfully", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<string> { Success = false, Message = "Failed to delete message", Error = ex.InnerException.Message, StatusCode = 500 };
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
                    return new HttpResponse<string> { Success = true, Message = $"User with the id: {id} was removed", StatusCode = 200 };
                }
                return new HttpResponse<string> { Success = false, Message = $"User with the id: {id} was't found", StatusCode = 404 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<string> { Success = false, Message = "Failed to deleted user", Error = ex.Message, StatusCode = 500 };
            }

        }

        public async Task<HttpResponse<User>> UpdateUserInformation(UpdateInformationDto updateInformationDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(updateInformationDto.Id);
                if (user == null)
                {
                    return new HttpResponse<User> { Success = false, Message = "Information Update Failed", Error = "The Id was not found in the database", StatusCode = 404 };
                }
                user.UpdateInformation(updateInformationDto);
                await _context.SaveChangesAsync();
                return new HttpResponse<User> { Success = true, Message = "Information updated successfully", StatusCode = 200 , Data= user};
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlException && IsDuplicateKeyError(sqlException))
                {
                    string errorMessage = GetDuplicateKeyErrorMessage(sqlException);
                    return new HttpResponse<User> { Success = false, Message = "Failed to update information", Error = errorMessage, StatusCode = 400 };
                }

                return new HttpResponse<User> { Success = false, Message = "Failed to update information", Error = ex.InnerException.Message, StatusCode = 500 };
            }
            catch (Exception ex)
            {
                return new HttpResponse<User> { Success = false, Message = "Failed to update information", Error = ex.InnerException.Message, StatusCode = 500 };
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
