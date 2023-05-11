using System.ComponentModel.DataAnnotations;

namespace Auth.Entities.DataTransferObjects
{
    public class CreateReviewRequestDto
    {
        public string? Title { get; set; }

        [Required]
        public string? Email { get; set; }
        public string? Description { get; set; }
        public string? ReviewText { get; set; }
        public string? Theme { get; set; }
        public string? Image { get; set; }
    }
}
