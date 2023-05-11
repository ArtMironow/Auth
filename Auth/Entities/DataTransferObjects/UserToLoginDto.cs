using System.ComponentModel.DataAnnotations;

namespace Auth.Entities.DataTransferObjects
{
    public class UserToLoginDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email is incorrect.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
    }
}
