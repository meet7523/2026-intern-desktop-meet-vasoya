using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Data
{
    public class DatabaseService
    {
        private readonly string _databasePath;
        private readonly string _connectionString;

        public DatabaseService()
        {
            // Store DB safely in: C:\Users\<Username>\AppData\Local\Blobstation\Ai Chat\
            string appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Blobstation",
                "Ai Chat"
            );

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            _databasePath = Path.Combine(appDataFolder, "AIChat.db");
            _connectionString = $"Data Source={_databasePath};Version=3;";

            // If AppData DB does not exist, but an existing AIChat.db is in the app directory, copy it over!
            string installedDb = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AIChat.db");
            if (!File.Exists(_databasePath) && File.Exists(installedDb))
            {
                try
                {
                    File.Copy(installedDb, _databasePath, true);
                }
                catch { }
            }
        }

        public SQLiteConnection GetConnection()
        {
            SQLiteConnection connection = new SQLiteConnection(_connectionString);
            connection.Open();

            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = "PRAGMA foreign_keys = ON;";
                command.ExecuteNonQuery();
            }

            return connection;
        }

        public bool ValidateUser(string userId, string password)
        {
            using (SQLiteConnection connection = GetConnection())
            {
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT COUNT(*)
                        FROM UserCredencial
                        WHERE UserId = @UserId
                          AND Password = @Password;
                    ";

                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Password", password);

                    object result = command.ExecuteScalar();
                    int count = Convert.ToInt32(result);

                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Registers a new user with UserId and Password.
        /// </summary>
        public bool RegisterUser(string userId, string password)
        {
            try
            {
                using (SQLiteConnection connection = GetConnection())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            INSERT OR IGNORE INTO UserCredencial (UserId, Password)
                            VALUES (@UserId, @Password);
                        ";
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@Password", password);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public int SaveOrUpdateGoogleUser(
            string googleUserId,
            string name,
            string email,
            string profilePicture)
        {
            using (SQLiteConnection connection = GetConnection())
            {
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO Users
                        (
                            GoogleUserId,
                            Name,
                            Email,
                            ProfilePicture,
                            CreatedDate,
                            LastLoginDate
                        )
                        VALUES
                        (
                            @GoogleUserId,
                            @Name,
                            @Email,
                            @ProfilePicture,
                            @CreatedDate,
                            @LastLoginDate
                        )
                        ON CONFLICT(GoogleUserId)
                        DO UPDATE SET
                            Name = excluded.Name,
                            Email = excluded.Email,
                            ProfilePicture = excluded.ProfilePicture,
                            LastLoginDate = excluded.LastLoginDate;

                        SELECT Id
                        FROM Users
                        WHERE GoogleUserId = @GoogleUserId;
                    ";

                    string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    command.Parameters.AddWithValue("@GoogleUserId", googleUserId);
                    command.Parameters.AddWithValue("@Name", name ?? string.Empty);
                    command.Parameters.AddWithValue("@Email", email ?? string.Empty);
                    command.Parameters.AddWithValue("@ProfilePicture", (object)profilePicture ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedDate", now);
                    command.Parameters.AddWithValue("@LastLoginDate", now);

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        private void AddAttachmentColumns(SQLiteConnection connection)
        {
            AddColumnIfNotExists(connection, "Messages", "AttachmentFileName", "TEXT NULL");
            AddColumnIfNotExists(connection, "Messages", "AttachmentExtension", "TEXT NULL");
            AddColumnIfNotExists(connection, "Messages", "AttachmentSize", "TEXT NULL");
            AddColumnIfNotExists(connection, "Messages", "AttachmentData", "BLOB NULL");
        }

        private void AddChatUserColumn(SQLiteConnection connection)
        {
            AddColumnIfNotExists(connection, "Chats", "UserId", "INTEGER NULL");
        }

        private void AddColumnIfNotExists(
            SQLiteConnection connection,
            string tableName,
            string columnName,
            string columnDefinition)
        {
            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = $"PRAGMA table_info({tableName});";

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string existingColumn = reader["name"].ToString();
                        if (string.Equals(existingColumn, columnName, StringComparison.OrdinalIgnoreCase))
                        {
                            return;
                        }
                    }
                }
            }

            using (SQLiteCommand command = connection.CreateCommand())
            {
                command.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnDefinition};";
                command.ExecuteNonQuery();
            }
        }

        public void InitializeDatabase()
        {
            try
            {
                if (!File.Exists(_databasePath))
                {
                    SQLiteConnection.CreateFile(_databasePath);
                }

                using (SQLiteConnection connection = GetConnection())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS Users
                            (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                GoogleUserId TEXT NOT NULL UNIQUE,
                                Name TEXT NOT NULL,
                                Email TEXT NOT NULL,
                                ProfilePicture TEXT NULL,
                                CreatedDate TEXT NOT NULL,
                                LastLoginDate TEXT NOT NULL
                            );

                            CREATE TABLE IF NOT EXISTS UserCredencial
                            (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                UserId TEXT NOT NULL UNIQUE,
                                Password TEXT NOT NULL
                            );
                            
                            CREATE TABLE IF NOT EXISTS Chats
                            (
                                ChatId INTEGER PRIMARY KEY AUTOINCREMENT,
                                UserId INTEGER NULL,
                                Title TEXT NOT NULL,
                                CreatedDate TEXT NOT NULL
                            );

                            CREATE TABLE IF NOT EXISTS Messages
                            (
                                MessageId INTEGER PRIMARY KEY AUTOINCREMENT,
                                ChatId INTEGER NOT NULL,
                                SenderType TEXT NOT NULL,
                                MessageText TEXT NOT NULL,
                                Timestamp TEXT NOT NULL,
                                AttachmentFileName TEXT NULL,
                                AttachmentExtension TEXT NULL,
                                AttachmentSize TEXT NULL,
                                AttachmentData BLOB NULL,
                                FOREIGN KEY (ChatId)
                                    REFERENCES Chats(ChatId)
                                    ON DELETE CASCADE
                            );
                        ";

                        command.ExecuteNonQuery();
                    }

                    AddAttachmentColumns(connection);
                    AddChatUserColumn(connection);

                    // SEED DEFAULT LOGIN CREDENTIALS IF TABLE IS EMPTY
                    SeedDefaultUserIfEmpty(connection);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to initialize SQLite database.", ex);
            }
        }

        private void SeedDefaultUserIfEmpty(SQLiteConnection connection)
        {
            using (SQLiteCommand checkCmd = connection.CreateCommand())
            {
                checkCmd.CommandText = "SELECT COUNT(*) FROM UserCredencial;";
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    using (SQLiteCommand insertCmd = connection.CreateCommand())
                    {
                        // Insert your default test credentials here:
                        insertCmd.CommandText = @"
                            INSERT INTO UserCredencial (UserId, Password)
                            VALUES ('Admin', 'Admin@12');
                        ";
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public bool TestConnection()
        {
            try
            {
                using (SQLiteConnection connection = GetConnection())
                {
                    return connection.State == System.Data.ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        public string DatabasePath => _databasePath;
    }
}