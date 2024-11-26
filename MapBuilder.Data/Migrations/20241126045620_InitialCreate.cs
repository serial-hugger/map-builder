using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapBuilder.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GenerationVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    GenerationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CellToken = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cells", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Feature",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WayId = table.Column<long>(type: "INTEGER", nullable: false),
                    TotalPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Closed = table.Column<bool>(type: "INTEGER", nullable: false),
                    CellId = table.Column<int>(type: "INTEGER", nullable: false),
                    RetrievedData = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feature_Cells_CellId",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeaturePoint",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PointId = table.Column<long>(type: "INTEGER", nullable: true),
                    WayId = table.Column<long>(type: "INTEGER", nullable: true),
                    PointOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Lat = table.Column<double>(type: "REAL", nullable: false),
                    Lng = table.Column<double>(type: "REAL", nullable: false),
                    CellId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeaturePoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeaturePoint_Cells_CellId",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feature_CellId",
                table: "Feature",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "IX_FeaturePoint_CellId",
                table: "FeaturePoint",
                column: "CellId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feature");

            migrationBuilder.DropTable(
                name: "FeaturePoint");

            migrationBuilder.DropTable(
                name: "Cells");
        }
    }
}
