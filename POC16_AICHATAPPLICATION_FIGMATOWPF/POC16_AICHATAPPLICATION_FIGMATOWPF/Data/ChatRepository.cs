using POC16_AICHATAPPLICATION_FIGMATOWPF.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC16_AICHATAPPLICATION_FIGMATOWPF.Data
{
    public class ChatRepository
    {
    
        private readonly DatabaseService _databaseService;
        public ChatRepository(DatabaseService databaseService)
        {
                _databaseService = databaseService;
        }


        // CREATE CHAT


        public int CreateChat(int userId,string title)
        {
            using (SQLiteConnection connection =
                   _databaseService.GetConnection())
            {


                using (SQLiteCommand command =
                           connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO Chats
                        (
                            UserId,
                            Title,
                            CreatedDate
                        )
                        VALUES
                        (
                            @UserId,
                            @Title,
                            @CreatedDate
                        );

                        SELECT last_insert_rowid();
                    ";

                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId);

                    command.Parameters.AddWithValue(
                        "@Title",
                        title);

                    command.Parameters.AddWithValue(
                        "@CreatedDate",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    return Convert.ToInt32(
                        command.ExecuteScalar());
                }
            }
        }


        // GET ALL CHATS


        public List<Chat> GetChats(int userId)
        {
                List<Chat> chats = new List<Chat>();

                using (SQLiteConnection connection =
                       _databaseService.GetConnection())
                {
                   

                    using (SQLiteCommand command =
                           connection.CreateCommand())
                    {
                        command.CommandText = @"
                         SELECT
                        ChatId,
                        Title,
                        CreatedDate
                        FROM Chats
                        WHERE UserId = @UserId
                        ORDER BY CreatedDate DESC;
                    ";


                    command.Parameters.AddWithValue(
                      "@UserId",
                      userId);

                    using (SQLiteDataReader reader =
                               command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Chat chat = new Chat
                                {
                                    ChatId = Convert.ToInt32(
                                        reader["ChatId"]),

                                    Title = reader["Title"].ToString(),

                                    CreatedDate = DateTime.Parse(
                                        reader["CreatedDate"].ToString())
                                };

                                chats.Add(chat);
                            }
                        }
                    }
                }

                return chats;
            }

            
            // GET CHAT BY ID
            

            public Chat GetChatById(int chatId , int userId)
            {
                using (SQLiteConnection connection =
                       _databaseService.GetConnection())
                {
                    

                    using (SQLiteCommand command =
                           connection.CreateCommand())
                    {
                        command.CommandText = @"
                        SELECT
                            ChatId,
                            Title,
                            CreatedDate
                        FROM Chats
                        WHERE ChatId = @ChatId
                        AND UserId = @UserId;
                    ";

                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId);

                    command.Parameters.AddWithValue(
                            "@ChatId",
                            chatId);

                        using (SQLiteDataReader reader =
                               command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Chat
                                {
                                    ChatId = Convert.ToInt32(
                                        reader["ChatId"]),

                                    Title = reader["Title"].ToString(),

                                    CreatedDate = DateTime.Parse(
                                        reader["CreatedDate"].ToString())
                                };
                            }
                        }
                    }
                }

                return null;
            }

            
            // UPDATE CHAT TITLE
            

            public bool UpdateChatTitle(
                int chatId,
                int userId,
                string title)
            {
                using (SQLiteConnection connection =
                       _databaseService.GetConnection())
                {
                    

                    using (SQLiteCommand command =
                           connection.CreateCommand())
                    {
                        command.CommandText = @"
                        UPDATE Chats
                        SET Title = @Title
                        WHERE ChatId = @ChatId
                        AND UserId = @UserId;
                    ";

                    command.Parameters.AddWithValue(
                    "@UserId",
                    userId);

                    command.Parameters.AddWithValue(
                            "@Title",
                            title);

                        command.Parameters.AddWithValue(
                            "@ChatId",
                            chatId);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }


        // DELETE CHAT


        public bool DeleteChat(int chatId, int userId)
        {
            using (SQLiteConnection connection =
                   _databaseService.GetConnection())
            {
                    

                using (SQLiteCommand command =
                       connection.CreateCommand())
                {
                 command.CommandText = @"
                DELETE FROM Chats
                WHERE ChatId = @ChatId
                AND UserId = @UserId;
                 ";

                    command.Parameters.AddWithValue(
                        "@ChatId",
                        chatId);

                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }


        // SAVE MESSAGE


        public int SaveMessage(
                int chatId,
                int userId,
                string senderType,
                string messageText,
                string attachmentFileName = null,
                string attachmentExtension = null,
                string attachmentSize = null,
                byte[] attachmentData = null)
            {
                using (SQLiteConnection connection =
                       _databaseService.GetConnection())
                {


                using (SQLiteCommand checkCommand =
                        connection.CreateCommand())
                {
                    checkCommand.CommandText = @"
                    SELECT COUNT(*)
                    FROM Chats
                    WHERE ChatId = @ChatId
                      AND UserId = @UserId;
    ";

                    checkCommand.Parameters.AddWithValue(
                        "@ChatId",
                        chatId);

                    checkCommand.Parameters.AddWithValue(
                        "@UserId",
                        userId);

                    int count =
                        Convert.ToInt32(
                            checkCommand.ExecuteScalar());

                    if (count == 0)
                    {
                        throw new UnauthorizedAccessException(
                            "This chat does not belong to the authenticated user.");
                    }
                }


                using (SQLiteCommand command =
                           connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO Messages
                        (
                            ChatId,
                            SenderType,
                            MessageText,
                            Timestamp,
                            AttachmentFileName,
                            AttachmentExtension,
                            AttachmentSize,
                            AttachmentData
                        )
                        VALUES
                        (
                            @ChatId,
                            @SenderType,
                            @MessageText,
                            @Timestamp,
                            @AttachmentFileName,        
                            @AttachmentExtension,   
                            @AttachmentSize,    
                            @AttachmentData
                        );

                        SELECT last_insert_rowid();
                    ";

                        command.Parameters.AddWithValue(
                            "@ChatId",
                            chatId);

                        command.Parameters.AddWithValue(
                            "@SenderType",
                            senderType);

                        command.Parameters.AddWithValue(
                            "@MessageText",
                            messageText);

                        command.Parameters.AddWithValue(
                            "@Timestamp",
                            DateTime.Now.ToString(
                                "yyyy-MM-dd HH:mm:ss"));

                    command.Parameters.AddWithValue(
            "@AttachmentFileName",
            (object)attachmentFileName ?? DBNull.Value);

                    command.Parameters.AddWithValue(
                        "@AttachmentExtension",
                        (object)attachmentExtension ?? DBNull.Value);

                    command.Parameters.AddWithValue(
                        "@AttachmentSize",
                        (object)attachmentSize ?? DBNull.Value);

                    SQLiteParameter attachmentParameter =
                        command.Parameters.Add(
                            "@AttachmentData",
                            System.Data.DbType.Binary);

                    attachmentParameter.Value =
                        (object)attachmentData ?? DBNull.Value;

                    return Convert.ToInt32(
                            command.ExecuteScalar());
                    }
                }
            }

            
            // GET MESSAGES
            

            public List<Message> GetMessages(int chatId, int userId)
            {
                List<Message> messages =
                    new List<Message>();

                using (SQLiteConnection connection =
                       _databaseService.GetConnection())
                {
                    

                    using (SQLiteCommand command =
                           connection.CreateCommand())
                    {
                        command.CommandText = @"
                        SELECT
                            m.MessageId,
                            m.ChatId,
                            m.SenderType,
                            m.MessageText,
                            m.Timestamp,
                            m.AttachmentFileName,
                            m.AttachmentExtension,
                            m.AttachmentSize,
                            m.AttachmentData

                        FROM Messages m
                        INNER JOIN Chats c
                            ON m.ChatId = c.ChatId
                        WHERE m.ChatId = @ChatId
                          AND c.UserId = @UserId
                        ORDER BY m.Timestamp ASC,
                                 m.MessageId ASC;
                    ";

                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId);

                    command.Parameters.AddWithValue(
                            "@ChatId",
                            chatId);

                        using (SQLiteDataReader reader =
                               command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Message message = new Message
                                {
                                    MessageId = Convert.ToInt32(
                                        reader["MessageId"]),

                                    ChatId = Convert.ToInt32(
                                        reader["ChatId"]),

                                    SenderType =
                                        reader["SenderType"].ToString(),

                                    MessageText =
                                        reader["MessageText"].ToString(),

                                    Timestamp =
                                        DateTime.Parse(
                                            reader["Timestamp"].ToString()),


                                    AttachmentFileName =
                                reader["AttachmentFileName"] == DBNull.Value
                                    ? null
                                    : reader["AttachmentFileName"].ToString(),

                                    AttachmentExtension =
                                reader["AttachmentExtension"] == DBNull.Value
                                    ? null
                                    : reader["AttachmentExtension"].ToString(),

                                    AttachmentSize =
                                reader["AttachmentSize"] == DBNull.Value
                                    ? null
                                    : reader["AttachmentSize"].ToString(),

                                    AttachmentData =
                                reader["AttachmentData"] == DBNull.Value
                                    ? null
                                    : (byte[])reader["AttachmentData"]
                                };

                                messages.Add(message);
                            }
                        }
                    }
                }

                return messages;
            }

            
            // DELETE MESSAGE
            

            public bool DeleteMessage(int messageId, int userId)
            {
                using (SQLiteConnection connection =
                       _databaseService.GetConnection())
                {
                    

                    using (SQLiteCommand command =
                           connection.CreateCommand())
                    {
                        command.CommandText = @"
                        DELETE FROM Messages
                        WHERE MessageId = @MessageId
                        AND ChatId IN
                        (
                            SELECT ChatId
                            FROM Chats
                            WHERE UserId = @UserId
                        );
                    ";

                    command.Parameters.AddWithValue(
                    "@UserId",
                    userId);

                    command.Parameters.AddWithValue(
                            "@MessageId",
                            messageId);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }

       
    }
    }
    

