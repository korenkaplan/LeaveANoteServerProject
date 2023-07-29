namespace LeaveANoteServerProject.Dto_s
{
    public class HttpResponse<T>
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
