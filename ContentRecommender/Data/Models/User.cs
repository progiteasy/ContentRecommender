using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContentRecommender.Data.Models
{
    [Table("Users")]
    public class User : IdentityUser
    {
        [Column("RegistrationDate", TypeName = "datetime2"), Required]
        public DateTime RegistrationDate { get; set; }

        [Column("IsActive", TypeName = "bit"), Required]
        public bool IsActive { get; set; }

        [InverseProperty("Author")]
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        [InverseProperty("User")]
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();

        [InverseProperty("User")]
        public ICollection<Like> Likes { get; set; } = new List<Like>();

        [InverseProperty("Author")]
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
