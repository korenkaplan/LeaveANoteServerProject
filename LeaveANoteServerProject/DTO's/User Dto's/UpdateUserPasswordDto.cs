namespace LeaveANoteServerProject.DTO_s.User_Dto_s
{
    public class UpdateUserPasswordDto
    {

        public int Id { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
    }
}
