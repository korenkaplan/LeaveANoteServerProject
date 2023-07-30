using LeaveANoteServerProject.Dto_s.User_Dto_s;
using LeaveANoteServerProject.DTO_s.User_Dto_s;
using Microsoft.Data.SqlClient;

namespace LeaveANoteServerProject.Services.UserService
{
    public interface IUserService
    {
        /// <summary>
        /// Register a new user in the database.
        /// </summary>
        /// <param name="user">The user object containing registration details.</param>
        /// <returns>A response with the registration status and a token if successful.</returns>
        Task<HttpResponse<object>> RegisterUser(User user);

        /// <summary>
        /// Authenticate a user and generate an access token.
        /// </summary>
        /// <param name="loginDto">The login information provided by the user.</param>
        /// <returns>A response with the authentication status and the access token if successful.</returns>
        Task<HttpResponse<object>> Login(LoginDto loginDto);

        /// <summary>
        /// Retrieve a list of all users from the database.
        /// </summary>
        /// <returns>A response with the list of users if successful.</returns>
        Task<HttpResponse<List<User>>> GetAllUsers();

        /// <summary>
        /// Update the device token for a user in the database.
        /// </summary>
        /// <param name="deviceTokenUpdateDto">The object containing the user ID and updated device token.</param>
        /// <returns>A response with the status of the token update.</returns>
        Task<HttpResponse<string>> UpdateDeviceToken(DeviceTokenUpdateDto deviceTokenUpdateDto);

        /// <summary>
        /// Update user information in the database.
        /// </summary>
        /// <param name="updateInformationDto">The object containing the updated user information.</param>
        /// <returns>A response with the status of the information update.</returns>
        Task<HttpResponse<User>> UpdateUserInformation(UpdateInformationDto updateInformationDto);

        /// <summary>
        /// Update the password for a user in the database.
        /// </summary>
        /// <param name="updateUserPasswordDto">The object containing the user ID, old password, and new password.</param>
        /// <returns>A response with the status of the password update.</returns>
        Task<HttpResponse<string>> UpdateUserPassword(UpdateUserPasswordDto updateUserPasswordDto);

        /// <summary>
        /// Mark a message (accident) as read for a user.
        /// </summary>
        /// <param name="accidentDeleteDto">The object containing the user ID and accident ID to mark as read.</param>
        /// <returns>A response with the status of the message update.</returns>
        Task<HttpResponse<string>> DeleteAccidentFromInbox(AccidentDeleteDto accidentDeleteDto);

        /// <summary>
        /// Mark a message (accident) as deleted for a user.
        /// </summary>
        /// <param name="accidentDeleteDto">The object containing the user ID and accident ID to mark as deleted.</param>
        /// <returns>A response with the status of the message update.</returns>
        Task<HttpResponse<string>> DeleteAccidentFromHistory(AccidentDeleteDto accidentDeleteDto);

        /// <summary>
        /// Get a user from the database by their car number.
        /// </summary>
        /// <param name="carNumber">The car number of the user to retrieve.</param>
        /// <returns>A response with the user information if found.</returns>
        Task<HttpResponse<GetUserByCarNumberDto>> GetUserByCarNumber(string carNumber);

        /// <summary>
        /// Get a user from the database by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>A response with the user information if found.</returns>
        Task<HttpResponse<User>> GetUserById(int id);

        /// <summary>
        /// Delete a user from the database by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A response with the status of the user deletion.</returns>
        Task<HttpResponse<string>> DeleteUserById(int id);

        /// <summary>
        /// Get a minimal user object from the database by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>A response with the minimal user information if found.</returns>
        Task<HttpResponse<MinimalUserDto>> GetMinimalUserById(int id);

    }
}
