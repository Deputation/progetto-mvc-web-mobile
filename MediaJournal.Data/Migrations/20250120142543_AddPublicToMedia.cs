﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaJournal.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicToMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Media",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Media");
        }
    }
}
