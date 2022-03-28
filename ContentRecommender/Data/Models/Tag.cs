using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContentRecommender.Data.Models
{
    [Table("Tags")]
    public class Tag
    {
        [Column("Id", TypeName = "bigint"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("Name", TypeName = "nvarchar(50)"), Required]
        public string Name { get; set; }

        [InverseProperty("Tags")]
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
