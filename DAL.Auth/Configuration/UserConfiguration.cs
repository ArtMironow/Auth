using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Auth.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            builder.HasData(
                new User
                {
                    Email = "1@gmail.com",
                    PasswordHash = hasher.HashPassword(null, "A12345b$")
                }
            );
        }
    }
}
