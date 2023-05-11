using System.ComponentModel.DataAnnotations;

namespace Auth.Entities.DataTransferObjects
{
    public class UpdateReviewRequestDto
    {
        [Required]
        public string? Id { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ReviewText { get; set; }
        public string? Theme { get; set; }
        public string? Image { get; set; }
    }
}
