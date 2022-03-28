using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContentRecommender.Data.Models
{
    [Table("Images")]
    public class Image
    {
        [Column("Id", TypeName = "bigint"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("Link", TypeName = "varchar(max)"), Required]
        public string Link { get; set; }

        [Column("ReviewId", TypeName = "bigint"), Required]
        public long ReviewId { get; set; }

        [ForeignKey("ReviewId")]
        public Review Review { get; set; }
    }
}
