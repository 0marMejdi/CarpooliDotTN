using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CarpooliDotTN.Models;

public class CarpooliDbContext : IdentityDbContext<User>
{
    public CarpooliDbContext(DbContextOptions<CarpooliDbContext> options) :base(options){ }
        public DbSet<Carpool> carpools { get; set; }
        public DbSet<Demand?> demands { get; set; }
    
 

        
    /**
     * this function is made for those who run this app first time in their local repository
     * it's to check if there a database under name "CarpooliDotTn" or no
     * if it does not exist it creates a one before connecting to it without any errors and exceptions
     * otherwise it does nothing. keep it in case someone tries to download and try it himself
     */
  
    public static void InitializeDatabase()
    {
        void CreateDatabase(SqlConnection connection, string databaseName)
        {
            using (SqlCommand command = new SqlCommand($"CREATE DATABASE {databaseName}", connection))
            {
                command.ExecuteNonQuery();
            }
        }
        bool DatabaseExists(SqlConnection connection, string databaseName)
        {
            using (SqlCommand command = new SqlCommand($"IF DB_ID('{databaseName}') IS NOT NULL SELECT 1 ELSE SELECT 0", connection))
            {
                return (int)command.ExecuteScalar() == 1;
            }
        }
        const string connectionStringMaster = "Data Source=(localdb)\\mssqllocaldb; Integrated Security=True";
        const string databaseName = "CarpooliDotTN";
        try
        {
            
            using (SqlConnection connection = new SqlConnection(connectionStringMaster))
            {
                
                connection.Open();
                // Check if the database exists
                if (DatabaseExists(connection, databaseName))
                {
                    Console.WriteLine("Database check completed, no actions needed.");
                }
                else
                {
                    // Create the database
                    CreateDatabase(connection, databaseName);
                    Console.WriteLine("Database not found, new one has been created.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}