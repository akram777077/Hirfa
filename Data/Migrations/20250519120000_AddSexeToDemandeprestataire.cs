using Microsoft.EntityFrameworkCore.Migrations;

namespace Hirfa.Web.Migrations
{
    public partial class AddSexeToDemandeprestataire : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "sexe",
                table: "demandeprestataire",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "M"); // Default to 'M' for existing rows, adjust as needed
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sexe",
                table: "demandeprestataire");
        }
    }
}
