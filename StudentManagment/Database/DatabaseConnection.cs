using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace StudentManagementApp.Database;

public static class DatabaseConnection
{
    private static readonly IConfiguration Configuration;

    static DatabaseConnection()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: false)
            .Build();
    }

    public static SqlConnection GetConnection()
    {
        string? connectionString =
            Configuration.GetConnectionString("StudentManagementDB");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "StudentManagementDB connection string was not found.");
        }

        return new SqlConnection(connectionString);
    }
}