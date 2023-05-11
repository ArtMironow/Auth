using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Auth.Models
{
    public class Like
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }

        public Guid? ReviewId { get; set; }
        public Review? Review { get; set; }
    }
}
