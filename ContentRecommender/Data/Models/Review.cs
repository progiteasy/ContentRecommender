using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContentRecommender.Data.Models
{
    [Table("Reviews")]
    public class Review
    {
        [Column("Id", TypeName = "bigint"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("CreationDate", TypeName = "datetime2"), Required]
        public DateTime CreationDate { get; set; }

        [Column("Title", TypeName = "nvarchar(300)"), Required]
        public string Title { get; set; }

        [Column("Text", TypeName = "nvarchar(max)"), Required]
        public string Text { get; set; }

        [Column("CategoryId", TypeName = "smallint"), Required]
        public short CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Column("AuthorId", TypeName = "nvarchar(450)"), Required]
        public string AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public User Author { get; set; }

        [InverseProperty("Review")]
        public ICollection<Image> Images { get; set; } = new List<Image>();

        [InverseProperty("Reviews")]
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();

        [InverseProperty("Review")]
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();

        [InverseProperty("Review")]
        public ICollection<Like> Likes { get; set; } = new List<Like>();

        [InverseProperty("Review")]
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
