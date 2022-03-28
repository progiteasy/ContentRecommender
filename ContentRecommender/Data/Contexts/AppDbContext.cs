using ContentRecommender.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;

namespace ContentRecommender.Data.Contexts
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

            modelBuilder.Entity<Rating>().HasOne(rating => rating.User).WithMany(user => user.Ratings).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Like>().HasOne(like => like.User).WithMany(user => user.Likes).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Comment>().HasOne(comment => comment.Author).WithMany(user => user.Comments).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>().HasMany(review => review.Tags).WithMany(tag => tag.Reviews).
                UsingEntity<Dictionary<string, object>>("ReviewTags",
                    review => review.HasOne<Tag>().WithMany().HasForeignKey("TagId").OnDelete(DeleteBehavior.Restrict),
                    tag => tag.HasOne<Review>().WithMany().HasForeignKey("ReviewId").OnDelete(DeleteBehavior.Restrict),
                    resultingEntity => resultingEntity.ToTable("ReviewTags"));
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
