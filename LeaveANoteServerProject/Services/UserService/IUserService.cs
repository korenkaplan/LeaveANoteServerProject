namespace LeaveANoteServerProject.Services.UserService
{
    public interface IUserService
    {
        Task<HttpResponse<object>> RegisterUser(User user);
        Task<HttpResponse<object>> Login(string email, string password);
        Task<HttpResponse<List<User>>> GetAllUsers();
    }
}
