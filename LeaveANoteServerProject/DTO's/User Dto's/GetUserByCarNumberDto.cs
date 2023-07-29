using Microsoft.Identity.Client;

namespace LeaveANoteServerProject.DTO_s.User_Dto_s
{
    public class GetUserByCarNumberDto
    {
        public GetUserByCarNumberDto(int id, string deviceToken)
        {
            Id = id;
            DeviceToken = deviceToken;
        }

        public int Id { get; set; }
      public string DeviceToken { get; set; }
        
    }
}
