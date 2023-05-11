using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Auth.Models
{
    public class Review
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ReviewText { get; set; }
        public string? Theme { get; set; }
        public string? Image { get; set; }
        public DateTime? Created { get; set; }
        public string? Link { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Rating> Ratings { get; set; }

        public ICollection<Like> Likes { get; set; }
    }
}
