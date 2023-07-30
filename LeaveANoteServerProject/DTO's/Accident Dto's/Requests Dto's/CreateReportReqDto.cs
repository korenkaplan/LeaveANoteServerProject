namespace LeaveANoteServerProject.DTO_s.Accident_Dto_s
{
    public class CreateReportReqDto
    {
        public string DamagedCarNumber { get; set; }
        public string HittingCarNumber { get; set; }
        public string ImageSource { get; set; }
        public bool IsAnonymous { get; set; }
        public ReporterDto Reporter { get; set; }
    }
}
