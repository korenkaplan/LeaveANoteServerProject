using LeaveANoteServerProject.Dto_s.User_Dto_s;
using LeaveANoteServerProject.DTO_s.User_Dto_s;

namespace LeaveANoteServerProject.Services.UserService
{
    public interface IUserService
    {
        Task<HttpResponse<object>> RegisterUser(User user);
        Task<HttpResponse<object>> Login(LoginDto loginDto);
        Task<HttpResponse<List<User>>> GetAllUsers();
        Task<HttpResponse<string>> UpdateDeviceToken(DeviceTokenUpdateDto deviceTokenUpdateDto);
        Task<HttpResponse<string>> UpdateUserInformation(UpdateInformationDto updateInformationDto);
        Task<HttpResponse<string>> UpdateUserPassword(UpdateUserPasswordDto updateUserPasswordDto);
        Task<HttpResponse<string>> DeleteAccidentFromInbox(AccidentDeleteDto accidentDeleteDto);
        Task<HttpResponse<string>> DeleteAccidentFromHistory(AccidentDeleteDto accidentDeleteDto);
        Task<HttpResponse<GetUserByCarNumberDto>> GetUserByCarNumber(string carNumber);
        Task<HttpResponse<User>> GetUserById(int id);
        Task<HttpResponse<string>> DeleteUserById(int id);
        Task<HttpResponse<MinimalUserDto>> GetMinimalUserById(int id);

    }
}
