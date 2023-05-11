namespace Auth.Entities.DataTransferObjects
{
    public class ResponseDto
    {
        public bool? IsSuccess { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public string? Data { get; set; }
    }
}
