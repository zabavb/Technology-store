namespace Client.Models
{
    public class Status
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }

        public Status(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
        public Status() { }
    }
}
