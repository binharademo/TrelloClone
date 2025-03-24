using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main()
    {
        // Test user credentials
        string username = "testuser";
        string email = "test@example.com";
        string password = "password123";

        // Hash the password using SHA256 (same as in AuthService)
        string passwordHash = HashPassword(password);

        // Connect to the database
        string connectionString = "Data Source=TrelloClone/trelloclone.db";
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        // Check if user already exists
        using (var checkCommand = connection.CreateCommand())
        {
            checkCommand.CommandText = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
            checkCommand.Parameters.AddWithValue("@Email", email);
            
            int count = Convert.ToInt32(checkCommand.ExecuteScalar());
            if (count > 0)
            {
                Console.WriteLine($"User with email {email} already exists. Updating password...");
                
                using var updateCommand = connection.CreateCommand();
                updateCommand.CommandText = "UPDATE Users SET PasswordHash = @PasswordHash WHERE Email = @Email";
                updateCommand.Parameters.AddWithValue("@PasswordHash", passwordHash);
                updateCommand.Parameters.AddWithValue("@Email", email);
                updateCommand.ExecuteNonQuery();
                
                Console.WriteLine("Password updated successfully!");
                return;
            }
        }

        // Insert the test user
        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
                INSERT INTO Users (Username, Email, PasswordHash)
                VALUES (@Username, @Email, @PasswordHash)";
            
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);
            
            command.ExecuteNonQuery();
        }

        Console.WriteLine($"Test user created successfully!");
        Console.WriteLine($"Username: {username}");
        Console.WriteLine($"Email: {email}");
        Console.WriteLine($"Password: {password}");
    }

    static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
