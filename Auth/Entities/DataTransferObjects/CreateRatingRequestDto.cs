using System.ComponentModel.DataAnnotations;

namespace Auth.Entities.DataTransferObjects
{
    public class CreateRatingRequestDto
    {
        [Required]
        public int? Value { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? ReviewId { get; set; }
    }
}
