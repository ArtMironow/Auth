using System.ComponentModel.DataAnnotations;

namespace Auth.Entities.DataTransferObjects
{
    public class UpdateRatingRequestDto
    {
        [Required]
        public string? Id { get; set; }

        [Required]
        public int? Value { get; set; }
    }
}
