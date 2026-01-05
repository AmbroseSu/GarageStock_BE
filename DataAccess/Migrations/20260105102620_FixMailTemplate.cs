using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixMailTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        ALTER TABLE ""MailTemplates""
        ALTER COLUMN ""TemplateCode"" TYPE integer
        USING
            CASE ""TemplateCode""
                WHEN 'SIGN_UP' THEN 0
                WHEN 'OTP' THEN 1
                WHEN 'RESET_PASSWORD' THEN 2
                ELSE 0
            END;
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        ALTER TABLE ""MailTemplates""
        ALTER COLUMN ""TemplateCode"" TYPE text
        USING
            CASE ""TemplateCode""
                WHEN 0 THEN 'SIGN_UP'
                WHEN 1 THEN 'OTP'
                WHEN 2 THEN 'RESET_PASSWORD'
                ELSE 'SIGN_UP'
            END;
    ");
        }
    }
}
