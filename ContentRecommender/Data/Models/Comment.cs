using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContentRecommender.Data.Models
{
    [Table("Comments")]
    public class Comment
    {
        [Column("Id", TypeName = "bigint"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("CreationDate", TypeName = "datetime2"), Required]
        public DateTime CreationDate { get; set; }

        [Column("Text", TypeName = "nvarchar(1000)"), Required]
        public string Text { get; set; }

        [Column("AuthorId", TypeName = "nvarchar(450)"), Required]
        public string AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public User Author { get; set; }

        [Column("ReviewId", TypeName = "bigint"), Required]
        public long ReviewId { get; set; }

        [ForeignKey("ReviewId")]
        public Review Review { get; set; }
    }
}
