namespace LeaveANoteServerProject.Dto_s
{
    public class HttpResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public T? Data { get; set; }
    }
}
