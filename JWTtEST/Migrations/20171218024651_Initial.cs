using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JWTtEST.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivationId = table.Column<string>(nullable: true),
                    ActivationSecret = table.Column<string>(nullable: true),
                    DeviceEndpointId = table.Column<string>(nullable: true),
                    RSAKeyXML = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<double>(nullable: false),
                    CreatedAsString = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DeviceEnpointId = table.Column<string>(nullable: true),
                    DeviceId = table.Column<long>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    HardwareId = table.Column<string>(nullable: true),
                    HardwareRevision = table.Column<string>(nullable: true),
                    Manufacturer = table.Column<string>(nullable: true),
                    ModelNumber = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Request = table.Column<string>(nullable: true),
                    Response = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true),
                    SharedSecret = table.Column<string>(nullable: true),
                    SharedSecretEncoded = table.Column<string>(nullable: true),
                    SoftwareRevision = table.Column<string>(nullable: true),
                    SoftwareVersion = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registrations_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_DeviceId",
                table: "Registrations",
                column: "DeviceId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
