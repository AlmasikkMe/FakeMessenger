using FakeMessenger.Core.Entities;
using Microsoft.Data.Sqlite;

namespace FakeMessenger.Core.DatabaseRepositories;

public class UsersRepository : BaseDatabaseRepository<User>
{
    private const string _connectionString = "Data Source=users.db";

    protected override void InitDatabase()
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """
            CREATE TABLE IF NOT EXISTS users 
            ( 
                Username TEXT NOT NULL PRIMARY KEY, 
                Firstname TEXT NOT NULL, 
                Lastname TEXT NOT NULL,
                IsDeleted INTEGER NOT NULL DEFAULT 0
            );
            """;

        command.ExecuteNonQuery();
    }

    public override User? GetValue(string username)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """SELECT Firstname, Lastname, IsDeleted FROM users WHERE Username = @username""";
        command.Parameters.AddWithValue("@username", username);

        using SqliteDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            string firstName = reader.GetString(0);
            string lastName = reader.GetString(1);

            return new(username, firstName, lastName) { IsDeleted = reader.GetBoolean(2) };
        }
        else return null;
    }

    public override IEnumerable<string> EnumerateKeys()
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """SELECT Username FROM users""";

        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            yield return reader.GetString(0);
        }
    }

    protected override void AddValue(User user)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """
            INSERT INTO users (Username, Firstname, Lastname, IsDeleted) 
            VALUES (@username, @firstName, @lastName, @isDeleted)
            """;
        command.Parameters.AddWithValue("@username", user.Username);
        command.Parameters.AddWithValue("@firstName", user.FirstName);
        command.Parameters.AddWithValue("@lastName", user.LastName);
        command.Parameters.AddWithValue("@isDeleted", user.IsDeleted);

        command.ExecuteNonQuery();
    }

    protected override void RemoveValue(string username)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """DELETE FROM users WHERE Username = @username""";
        command.Parameters.AddWithValue("@username", username);

        command.ExecuteNonQuery();
    }

    public override void Remove(User item)
    {
        base.Remove(item, item.Username);
    }
    
    protected override void ClearValues()
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """DELETE FROM users""";

        command.ExecuteNonQuery();
    }
}