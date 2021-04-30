using Microsoft.EntityFrameworkCore.Migrations;

namespace FakeNews.Migrations
{
    public partial class VotingSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfDislike",
                table: "News",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfLikes",
                table: "News",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfDislike",
                table: "News");

            migrationBuilder.DropColumn(
                name: "NumberOfLikes",
                table: "News");
        }
    }
}
