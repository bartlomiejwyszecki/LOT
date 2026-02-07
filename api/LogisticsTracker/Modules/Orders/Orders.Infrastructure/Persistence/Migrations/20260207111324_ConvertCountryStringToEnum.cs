using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orders.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConvertCountryStringToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add a temporary integer column to store enum values
            migrationBuilder.AddColumn<int>(
                name: "ShippingAddress_Country_temp",
                schema: "orders_schema",
                table: "Orders",
                type: "integer",
                nullable: true);

            // Migrate data from text to enum integer values
            // Map country code 'Polska' to POL enum value (0)
            migrationBuilder.Sql(@"
                UPDATE orders_schema.""Orders""
                SET ""ShippingAddress_Country_temp"" = 0
                WHERE ""ShippingAddress_Country"" = 'Polska';
            ");

            // Set default value (0 = POL) for any remaining NULL or unmapped values
            migrationBuilder.Sql(@"
                UPDATE orders_schema.""Orders""
                SET ""ShippingAddress_Country_temp"" = 0
                WHERE ""ShippingAddress_Country_temp"" IS NULL;
            ");

            // Drop the old text column
            migrationBuilder.DropColumn(
                name: "ShippingAddress_Country",
                schema: "orders_schema",
                table: "Orders");

            // Rename temporary column to the original name
            migrationBuilder.RenameColumn(
                name: "ShippingAddress_Country_temp",
                schema: "orders_schema",
                table: "Orders",
                newName: "ShippingAddress_Country");

            // Set the column to not null
            migrationBuilder.AlterColumn<int>(
                name: "ShippingAddress_Country",
                schema: "orders_schema",
                table: "Orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add temporary text column
            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_Country_temp",
                schema: "orders_schema",
                table: "Orders",
                type: "text",
                nullable: true);

            // Convert enum values back to string codes
            migrationBuilder.Sql(@"
                UPDATE orders_schema.""Orders""
                SET ""ShippingAddress_Country_temp"" = CASE 
                    -- Europe
                    WHEN ""ShippingAddress_Country"" = 0 THEN 'POL'
                    WHEN ""ShippingAddress_Country"" = 1 THEN 'DEU'
                    WHEN ""ShippingAddress_Country"" = 2 THEN 'FRA'
                    WHEN ""ShippingAddress_Country"" = 3 THEN 'GBR'
                    WHEN ""ShippingAddress_Country"" = 4 THEN 'ITA'
                    WHEN ""ShippingAddress_Country"" = 5 THEN 'ESP'
                    WHEN ""ShippingAddress_Country"" = 6 THEN 'NLD'
                    WHEN ""ShippingAddress_Country"" = 7 THEN 'BEL'
                    WHEN ""ShippingAddress_Country"" = 8 THEN 'AUT'
                    WHEN ""ShippingAddress_Country"" = 9 THEN 'CZE'
                    WHEN ""ShippingAddress_Country"" = 10 THEN 'SVK'
                    WHEN ""ShippingAddress_Country"" = 11 THEN 'HUN'
                    WHEN ""ShippingAddress_Country"" = 12 THEN 'ROU'
                    WHEN ""ShippingAddress_Country"" = 13 THEN 'BGR'
                    WHEN ""ShippingAddress_Country"" = 14 THEN 'HRV'
                    WHEN ""ShippingAddress_Country"" = 15 THEN 'SVN'
                    WHEN ""ShippingAddress_Country"" = 16 THEN 'SWE'
                    WHEN ""ShippingAddress_Country"" = 17 THEN 'NOR'
                    WHEN ""ShippingAddress_Country"" = 18 THEN 'DNK'
                    WHEN ""ShippingAddress_Country"" = 19 THEN 'FIN'
                    WHEN ""ShippingAddress_Country"" = 20 THEN 'GRC'
                    WHEN ""ShippingAddress_Country"" = 21 THEN 'PRT'
                    WHEN ""ShippingAddress_Country"" = 22 THEN 'IRL'
                    WHEN ""ShippingAddress_Country"" = 23 THEN 'LUX'
                    WHEN ""ShippingAddress_Country"" = 24 THEN 'MLT'
                    WHEN ""ShippingAddress_Country"" = 25 THEN 'CYP'
                    -- Asia
                    WHEN ""ShippingAddress_Country"" = 26 THEN 'CHN'
                    WHEN ""ShippingAddress_Country"" = 27 THEN 'JPN'
                    WHEN ""ShippingAddress_Country"" = 28 THEN 'IND'
                    WHEN ""ShippingAddress_Country"" = 29 THEN 'KOR'
                    WHEN ""ShippingAddress_Country"" = 30 THEN 'THA'
                    WHEN ""ShippingAddress_Country"" = 31 THEN 'VNM'
                    WHEN ""ShippingAddress_Country"" = 32 THEN 'MYS'
                    WHEN ""ShippingAddress_Country"" = 33 THEN 'SGP'
                    WHEN ""ShippingAddress_Country"" = 34 THEN 'IDN'
                    WHEN ""ShippingAddress_Country"" = 35 THEN 'PHL'
                    -- Americas
                    WHEN ""ShippingAddress_Country"" = 36 THEN 'USA'
                    WHEN ""ShippingAddress_Country"" = 37 THEN 'CAN'
                    WHEN ""ShippingAddress_Country"" = 38 THEN 'MEX'
                    WHEN ""ShippingAddress_Country"" = 39 THEN 'BRA'
                    WHEN ""ShippingAddress_Country"" = 40 THEN 'ARG'
                    WHEN ""ShippingAddress_Country"" = 41 THEN 'CHL'
                    -- Africa
                    WHEN ""ShippingAddress_Country"" = 42 THEN 'ZAF'
                    WHEN ""ShippingAddress_Country"" = 43 THEN 'EGY'
                    WHEN ""ShippingAddress_Country"" = 44 THEN 'NGA'
                    -- Oceania
                    WHEN ""ShippingAddress_Country"" = 45 THEN 'AUS'
                    WHEN ""ShippingAddress_Country"" = 46 THEN 'NZL'
                END;
            ");

            // Drop the integer column
            migrationBuilder.DropColumn(
                name: "ShippingAddress_Country",
                schema: "orders_schema",
                table: "Orders");

            // Rename temporary column back
            migrationBuilder.RenameColumn(
                name: "ShippingAddress_Country_temp",
                schema: "orders_schema",
                table: "Orders",
                newName: "ShippingAddress_Country");

            // Set the column to not null
            migrationBuilder.AlterColumn<string>(
                name: "ShippingAddress_Country",
                schema: "orders_schema",
                table: "Orders",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
