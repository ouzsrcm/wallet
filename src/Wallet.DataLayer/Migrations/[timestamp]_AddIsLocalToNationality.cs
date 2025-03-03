using Microsoft.EntityFrameworkCore.Migrations;

public partial class AddIsLocalToNationality : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsLocal",
            table: "Nationalities",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsLocal",
            table: "Nationalities");
    }
} 