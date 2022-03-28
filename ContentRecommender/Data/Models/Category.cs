using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContentRecommender.Data.Models
{
    [Table("Categories")]
    public class Category
    {
        [Column("Id", TypeName = "smallint"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short Id { get; set; }

        [Column("Name", TypeName = "nvarchar(50)"), Required]
        public string Name { get; set; }

        [InverseProperty("Category")]
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
