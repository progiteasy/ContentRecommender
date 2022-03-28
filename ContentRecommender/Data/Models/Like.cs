using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContentRecommender.Data.Models
{
    [Table("Likes")]
    public class Like
    {
        [Column("Id", TypeName = "bigint"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("UserId", TypeName = "nvarchar(450)"), Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Column("ReviewId", TypeName = "bigint"), Required]
        public long ReviewId { get; set; }

        [ForeignKey("ReviewId")]
        public Review Review { get; set; }
    }
}
