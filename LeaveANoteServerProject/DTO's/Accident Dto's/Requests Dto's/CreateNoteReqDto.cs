namespace LeaveANoteServerProject.DTO_s.Accident_Dto_s
{
    public class CreateNoteReqDto
    {
        public int UserId { get; set; }
        public string HittingDriverPhoneNumber { get; set; }
        public string HittingCarNumber { get; set; }
        public string HittingDriverName { get; set; }
        public string ImageSource { get; set; }
    }
}
