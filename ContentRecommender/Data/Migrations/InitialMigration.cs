using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentRecommender.Data.Migrations
{
    public class InitialMigrationBuilder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG ftCatalog AS DEFAULT;", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON Reviews(Text, Title) KEY INDEX PK_Reviews;", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON Comments(Text) KEY INDEX PK_Comments;", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON Tags(Name) KEY INDEX PK_Tags;", true);
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON Categories(Name) KEY INDEX PK_Categories;", true);
        }
    }
}
