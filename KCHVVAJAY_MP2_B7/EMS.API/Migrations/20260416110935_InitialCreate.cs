using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EMS.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CreatedAt", "Department", "Designation", "Email", "FirstName", "JoinDate", "LastName", "Phone", "Salary", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "Software Engineer", "chandu.ajay@gmail.com", "Chandu", new DateTime(2021, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ajay", "9876543210", 850000m, "Active" },
                    { 2, new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Marketing", "Marketing Executive", "sairam.kolavennu@gmail.com", "Sairam", new DateTime(2020, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kolavennu", "9823456701", 620000m, "Active" },
                    { 3, new DateTime(2024, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), "HR", "HR Executive", "neha.chaudhary@gmail.com", "Neha", new DateTime(2019, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chaudhary", "9812345678", 550000m, "Active" },
                    { 4, new DateTime(2024, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Finance", "Financial Analyst", "rohith.reddy@gmail.com", "Rohith", new DateTime(2022, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Reddy", "9834567890", 720000m, "Active" },
                    { 5, new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Operations", "Operations Manager", "ambika.kumari@gmail.com", "Ambika", new DateTime(2018, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kumari", "9845678901", 950000m, "Active" },
                    { 6, new DateTime(2024, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "Senior Developer", "vikram.rajputh@gmail.com", "Vikram", new DateTime(2017, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rajputh", "9856789012", 1100000m, "Active" },
                    { 7, new DateTime(2024, 1, 7, 0, 0, 0, 0, DateTimeKind.Utc), "Marketing", "Content Strategist", "anusha.kapoor@gmail.com", "Anusha", new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kapoor", "9867890123", 580000m, "Inactive" },
                    { 8, new DateTime(2024, 1, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Finance", "Accounts Manager", "karthikeya.varma@gmail.com", "Karthikeya", new DateTime(2020, 4, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Varma", "9878901234", 800000m, "Active" },
                    { 9, new DateTime(2024, 1, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "Talent Acquisition Lead", "kavya.nikitha@gmail.com", "Kavya", new DateTime(2021, 8, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nikitha", "9889012345", 900000m, "Inactive" },
                    { 10, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Operations", "Logistics Coordinator", "mohan.lal@gmail.com", "Mohan", new DateTime(2019, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lal", "9890123456", 610000m, "Active" },
                    { 11, new DateTime(2024, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Marketing", "Brand Manager", "poorna.lakshmi@gmail.com", "Poorna", new DateTime(2021, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lakshmi", "9801234567", 780000m, "Active" },
                    { 12, new DateTime(2024, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Finance", "Tax Consultant", "mahesh.babu@gmail.com", "Mahesh", new DateTime(2022, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Babu", "9811234568", 690000m, "Inactive" },
                    { 13, new DateTime(2024, 1, 13, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "QA Engineer", "meera.joseph@gmail.com", "Meera", new DateTime(2022, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Joseph", "9822345679", 730000m, "Active" },
                    { 14, new DateTime(2024, 1, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "DevOps Engineer", "chandini.priya@gmail.com", "Chandini", new DateTime(2023, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Priya", "9833456780", 970000m, "Active" },
                    { 15, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Operations", "Supply Chain Analyst", "amit.narayana@gmail.com", "Amit", new DateTime(2020, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Narayana", "9844567891", 650000m, "Inactive" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$12$ZJEqfMbu9mbOHG5pSpJ4gu.IzAr04BVGF7GvQBqZdgggFGvpR9FtC", "Admin", "admin" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$12$0G0J3kJSc7mgjUXpykeQ6.gM196MwfkDpsw3BIQ.0fsNKx/E05EnS", "Viewer", "viewer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
