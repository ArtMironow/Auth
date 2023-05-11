using System.Text.Json.Nodes;

namespace Auth.Entities.DataTransferObjects
{
    public class AccountInfoResponseDto
    {
        public bool? IsSuccess { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public JsonObject? Data { get; set; }
    }
}
