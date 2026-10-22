using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;

namespace POC07_SQLiteDatabase_LibraryManagement
{
    internal class Program
    {

        class Books
        {
            public string isbn { get; set; }

            public string title { get; set; }
            public string author { get; set; }

            public string category { get; set; }

            public int publishedYear { get; set; }

            public int totalCopies { get; set; }
            public int availableCopies { get; set; }

            public DateTime dateAdded { get; set; }

            public int totalBooks { get; set; }

            public int totalMember { get; set; }

            public int borrowbook { get; set; }
            public int overduebook { get; set; }

            public int AvailableCopies { get; set; }
            public int borrowedCopies { get; set; }
        }

        class Members
        {
            public string Membercode { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public DateTime Membershipdate { get; set; }
            public int IsActive { get; set; }
        }

        class BorrowedRecord
        {
            public int BookId { get; set; }
            public int MemberId { get; set; }

            public DateTime BorrowDate { get; set; }

            public DateTime DueDate { get; set; }

            public DateTime? ReturnDate { get; set; }

            public decimal Fine { get; set; }


        }




        public class Library
        {
            Books b = new Books();
            Members m = new Members();
            BorrowedRecord br = new BorrowedRecord();

            private int publishedYear;


            public void AddNewBook()
            {
                try
                {
                    while (true)
                    {
                        Console.WriteLine("Enter ISBN :");
                        b.isbn = Console.ReadLine();


                        if (!IsValidISBN(b.isbn))
                        {
                            Console.WriteLine("Invalid ISBN!");
                            continue;
                        }
                        break;

                        bool IsValidISBN(string isbn)
                        {
                            isbn = isbn.Replace("-", "").Replace(" ", "");
                            return Regex.IsMatch(isbn, @"^\d{10}$|^\d{13}$");
                        }
                    }


                    while (true)
                    {
                        Console.WriteLine("Enter Title :");
                        b.title = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(b.title))
                        {
                            Console.WriteLine("Title Cannot NUll Or Empty");
                            continue;
                        }
                        break;
                    }

                    while (true)
                    {
                        Console.WriteLine("Enter Author :");
                        b.author = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(b.author))
                        {
                            Console.WriteLine("Author Cannot NUll Or Empty");
                            continue;
                        }
                        break;
                    }

                    while (true)
                    {
                        Console.WriteLine("Enter Category :");
                        b.category = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(b.category))
                        {
                            Console.WriteLine("Category Cannot NUll Or Empty");
                            continue;
                        }
                        break;
                    }

                    while (true)
                    {
                        Console.Write("Enter Published Year: ");

                        if (int.TryParse(Console.ReadLine(), out int publishedYear))
                        {
                            int currentYear = DateTime.Now.Year;

                            if (publishedYear >= 1900 && publishedYear <= currentYear)
                            {
                                b.publishedYear = publishedYear;
                                break;
                            }
                        }

                        Console.WriteLine("Invalid Year! Enter year between 1900 and current year.");
                    }

                    while (true)
                    {
                        Console.Write("Enter Total Copies: ");

                        if (int.TryParse(Console.ReadLine(), out int totalCopies)
                            && totalCopies > 0)
                        {
                            b.totalCopies = totalCopies;
                            break;
                        }

                        Console.WriteLine("Invalid Total Copies. Enter a positive number.");
                    }

                    while (true)
                    {
                        Console.Write("Enter Available Copies: ");

                        if (int.TryParse(Console.ReadLine(), out int availableCopies)
                            && availableCopies >= 0
                            && availableCopies <= b.totalCopies)
                        {
                            b.availableCopies = availableCopies;
                            break;
                        }

                        Console.WriteLine($"Available Copies must be between 0 and {b.totalCopies}.");
                    }

                    while (true)
                    {
                        Console.Write("Enter Date: ");

                        if (DateTime.TryParse(Console.ReadLine(), out DateTime dateAdded))
                        {
                            b.dateAdded = dateAdded;
                            break;
                        }

                        Console.WriteLine("Invalid Date Format.");
                    }



                    try
                    {
                        string cs = "Data Source=Library.db";
                        SQLiteConnection con = new SQLiteConnection(cs);

                        con.Open();

                        Console.WriteLine("Database Connection Successfully");


                        string createtable = @"

                        CREATE TABLE IF NOT EXISTS Books
                        (
                            BookId INTEGER PRIMARY KEY AUTOINCREMENT,
                            ISBN TEXT UNIQUE NOT NULL,
                            Title TEXT NOT NULL,
                            Author TEXT NOT NULL, 
                            Category TEXT,
                            PublishedYear INTEGER,
                            TotalCopies INTEGER DEFAULT 1,
                            AvailableCopies INTEGER DEFAULT 1,
                            DateAdded DATETIME DEFAULT CURRENT_TIMESTAMP
                        )";

                        SQLiteCommand fk = new SQLiteCommand("PRAGMA foreign_keys = ON;", con);

                        fk.ExecuteNonQuery();

                        SQLiteCommand createCmd = new SQLiteCommand(createtable, con);
                        createCmd.ExecuteNonQuery();

                        Console.WriteLine("Table Created.");


                        string insertsql = "INSERT INTO Books(ISBN,Title,Author,Category,PublishedYear,TotalCopies,AvailableCopies,DateAdded)VALUES(@Isbn,@Title,@Author,@Category,@PublishedYear,@TotalCopies,@AvailableCopies,@DateAdded)";

                        SQLiteCommand cmd = new SQLiteCommand(insertsql, con);

                        cmd.Parameters.AddWithValue("@Isbn", b.isbn);
                        cmd.Parameters.AddWithValue("@Title", b.title);
                        cmd.Parameters.AddWithValue("@Author", b.author);
                        cmd.Parameters.AddWithValue("@Category", b.category);
                        cmd.Parameters.AddWithValue("@PublishedYear", b.publishedYear);
                        cmd.Parameters.AddWithValue("@TotalCopies", b.totalCopies);
                        cmd.Parameters.AddWithValue("@AvailableCopies", b.availableCopies);
                        cmd.Parameters.AddWithValue("@DateAdded", b.dateAdded);

                        int row = cmd.ExecuteNonQuery();

                        if (row > 0)
                        {
                            Console.WriteLine("Book Added Successfully");
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine("Database Error:" + ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Input Fields Error" + ex.Message);
                }

            }


            public void ViewAllBooks()
            {
                Books b = new Books();

                Console.WriteLine("All Books In Library");
                Console.WriteLine("____________________________________________________________________________________________________________________\n");
                Console.WriteLine("ID\t ISBN\t\t Title\t\t Author\t\t AvailableCopies / TotalCopies \t Category");

                try
                {
                    string cs = "Data Source=Library.db";
                    SQLiteConnection con = new SQLiteConnection(cs);

                    con.Open();

                    string selectsql = "SELECT * FROM Books";

                    SQLiteCommand cmd = new SQLiteCommand(selectsql, con);

                    SQLiteDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No books found in Books table ");
                    }

                    while (reader.Read())
                    {
                        Console.WriteLine(
                            $"{reader["BookId"]}\t" +
                            $"{reader["ISBN"]}\t" +
                            $"{reader["Title"]}\t" +
                            $"{reader["Author"]}\t\t" +
                            $"{reader["AvailableCopies"]}\t\t{reader["TotalCopies"]}\t" +
                            $"{reader["Category"]}\t"

                        );

                    }
                    con.Close();

                    Console.WriteLine("---------------------------------------------------------------------------------------------------------------\n");

                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }


            public void SearchOperation()
            {
                Console.WriteLine("Enter Title,Author,ISBN, or Catagory to Search Book");
                string searchbook = Console.ReadLine();

                try
                {
                    string cs = "Data Source=Library.db";
                    SQLiteConnection con = new SQLiteConnection(cs);

                    con.Open();

                    string searchsql = @"SELECT * FROM Books where Title LIKE @Search or Author LIKE @Search or ISBN LIKE @Search or Category LIKE @Search";

                    SQLiteCommand cmd = new SQLiteCommand(searchsql, con);

                    cmd.Parameters.AddWithValue("@Search", "%" + searchbook + "%");

                    SQLiteDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No Book Found");
                    }

                    Console.WriteLine("ID\t ISBN\t\t Title\t\t Author\t\t Available\t Category");
                    Console.WriteLine("_____________________________________________________________\n");

                    while (reader.Read())
                    {
                        Console.WriteLine(
                            $"{reader["BookId"]}\t" +
                            $"{reader["ISBN"]}\t" +
                            $"{reader["Title"]}\t" +
                            $"{reader["Author"]}\t\t" +
                            $"{reader["AvailableCopies"]}/{reader["TotalCopies"]}\t" +
                            $"{reader["Category"]}\t"


                        );

                    }
                    con.Close();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error:" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }


            public void UpdateBook()
            {
                Books b = new Books();

                Console.WriteLine("Enter BookId to Update Books");
                int bookid = Convert.ToInt32(Console.ReadLine());

                try
                {
                    string cs = "Data Source=Library.db";
                    SQLiteConnection con = new SQLiteConnection(cs);

                    con.Open();

                    int option;

                    Console.WriteLine("\n------ Update Menu ------");
                    Console.WriteLine("1. ISBN");
                    Console.WriteLine("2. Title");
                    Console.WriteLine("3. Author");
                    Console.WriteLine("4. Category");
                    Console.WriteLine("5. Published Year");
                    Console.WriteLine("6. Total Copies");
                    Console.WriteLine("7. Available Copies");

                    Console.Write("Enter Choice: ");
                    option = Convert.ToInt32(Console.ReadLine());



                    SQLiteCommand scmd = null;

                    switch (option)
                    {
                        case 1:
                            Console.WriteLine("Enter new ISBN :");
                            b.isbn = Console.ReadLine();

                            string updateisbn = @"UPDATE Books SET ISBN = @ISBN Where BookId = @BookId";

                            scmd = new SQLiteCommand(updateisbn, con);

                            scmd.Parameters.AddWithValue("@ISBN", b.isbn);
                            scmd.Parameters.AddWithValue("@BookId", bookid);
                            break;

                        case 2:
                            Console.Write("Enter New Title: ");
                            b.title = Console.ReadLine();

                            string updatetitle = "UPDATE Books SET Title = @Title WHERE BookId=@BookId";
                            scmd = new SQLiteCommand(updatetitle, con);

                            scmd.Parameters.AddWithValue("@Title", b.title);
                            scmd.Parameters.AddWithValue("@BookId", bookid);
                            break;

                        case 3:
                            Console.Write("Enter New Author: ");
                            b.author = Console.ReadLine();

                            string updateauthor = "UPDATE Books SET Author = @Author WHERE BookId=@BookId";
                            scmd = new SQLiteCommand(updateauthor, con);

                            scmd.Parameters.AddWithValue("@Author", b.author);
                            scmd.Parameters.AddWithValue("@BookId", bookid);
                            break;

                        case 4:
                            Console.Write("Enter New Category: ");
                            b.category = Console.ReadLine();

                            string updatecatagory = "UPDATE Books SET Category = @Category WHERE BookId=@BookId";

                            scmd = new SQLiteCommand(updatecatagory, con);
                            scmd.Parameters.AddWithValue("@Category", b.category);
                            scmd.Parameters.AddWithValue("@BookId", bookid);
                            break;

                        case 5:
                            Console.Write("Enter New Published Year: ");
                            b.publishedYear = Convert.ToInt32(Console.ReadLine());

                            string updateyear = "UPDATE Books SET PublishedYear = @PublishedYear WHERE BookId=@BookId";
                            scmd = new SQLiteCommand(updateyear, con);

                            scmd.Parameters.AddWithValue("@PublishedYear", b.publishedYear);
                            scmd.Parameters.AddWithValue("@BookId", bookid);
                            break;

                        case 6:
                            Console.Write("Enter New Total Copies: ");
                            b.totalCopies = Convert.ToInt32(Console.ReadLine());

                            string updatetotalcopies = "UPDATE Books SET TotalCopies = @TotalCopies WHERE BookId=@BookId";

                            scmd = new SQLiteCommand(updatetotalcopies, con);
                            scmd.Parameters.AddWithValue("@TotalCopies", b.totalCopies);
                            scmd.Parameters.AddWithValue("@BookId", bookid);
                            break;

                        case 7:
                            Console.Write("Enter New Available Copies: ");
                            b.availableCopies = Convert.ToInt32(Console.ReadLine());

                            string updateavailablecopies = "UPDATE Books SET AvailableCopies = @AvailableCopies WHERE BookId=@BookId";

                            scmd = new SQLiteCommand(updateavailablecopies, con);
                            scmd.Parameters.AddWithValue("@AvailableCopies", b.availableCopies);
                            scmd.Parameters.AddWithValue("@BookId", bookid);
                            break;

                        default:
                            Console.WriteLine("Invalid Choice");
                            break;
                    }


                    int rows = scmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        Console.WriteLine("Book Updated Successfully");
                    }
                    else
                    {
                        Console.WriteLine("Book Not found");
                    }
                    con.Close();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error:" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }


            public void DeleteBooks()
            {
                Books b = new Books();

                try
                {

                    int bookId;

                    while (true)
                    {
                        Console.Write("Enter Book Id to Delete: ");

                        if (int.TryParse(Console.ReadLine(), out bookId) && bookId > 0)
                            break;

                        Console.WriteLine("Invalid Book Id.");
                    }

                    string cs = "Data Source=Library.db";

                    SQLiteConnection con = new SQLiteConnection(cs);

                    con.Open();

                    string checkBorrow = @"SELECT COUNT(*) FROM BorrowRecords WHERE BookId = @BookId";

                    SQLiteCommand checkCmd = new SQLiteCommand(checkBorrow, con);

                    checkCmd.Parameters.AddWithValue("@BookId", bookId);

                    int borrowCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (borrowCount > 0)
                    {
                        Console.WriteLine(
                            "Cannot delete book. This book has been borrowed before.");
                        return;
                    }

                    string deleteQuery = @"DELETE FROM Books WHERE BookId = @BookId";

                    SQLiteCommand deleteCmd = new SQLiteCommand(deleteQuery, con);

                    deleteCmd.Parameters.AddWithValue("@BookId", bookId);

                    int rowsAffected = deleteCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Book deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Book not found.");
                    }
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error:" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }




            public void AddMembers()
            {
                Members m = new Members();
                try
                {
                    while (true)
                    {
                        Console.WriteLine("Enter Member Unique Code with Text and Number");
                        m.Membercode = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(m.Membercode))
                        {
                            Console.WriteLine("MemberCode cannot Empty");
                            continue;
                        }
                        break;
                    }

                    while (true)
                    {
                        Console.Write("Enter Full Name: ");
                        m.FullName = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(m.FullName))
                        {
                            Console.WriteLine("Name cannot be empty.");
                            continue;
                        }

                        break;
                    }

                    while (true)
                    {
                        Console.Write("Enter Email: ");
                        m.Email = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(m.Email))
                        {
                            break;
                        }

                        if (!Regex.IsMatch(
                                m.Email,
                                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                        {
                            Console.WriteLine("Invalid Email Format.");
                            continue;
                        }

                        break;
                    }

                    while (true)
                    {
                        Console.Write("Enter Phone Number: ");
                        m.Phone = Console.ReadLine();

                        if (Regex.IsMatch(m.Phone, @"^\d{10}$"))
                            break;

                        Console.WriteLine("Phone must be 10 digits.");
                    }

                    while (true)
                    {
                        Console.Write("Enter Address: ");
                        m.Address = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(m.Address))
                        {
                            Console.WriteLine("Address Connot Empty");
                            continue;
                        }
                        break;
                    }

                    while (true)
                    {
                        Console.WriteLine("Enter MemberShip Dates");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime membershipDate))
                        {
                            m.Membershipdate = membershipDate;
                            break;
                        }

                        Console.WriteLine("Invalid Formate");
                    }

                    while (true)
                    {
                        Console.Write("Enter IsActive 0 or 1: ");

                        if (int.TryParse(Console.ReadLine(), out int active)
                            && (active == 0 || active == 1))
                        {
                            m.IsActive = active;
                            break;
                        }

                        Console.WriteLine("Enter only 0 or 1");
                    }



                    try
                    {
                        string cs = "Data Source=Library.db";

                        SQLiteConnection con = new SQLiteConnection(cs);

                        con.Open();

                        Console.WriteLine("Database Connection Successfully");

                        string createtable = @"

                            CREATE TABLE IF NOT EXISTS Members
                            (
                                MemberId INTEGER PRIMARY KEY AUTOINCREMENT,
                                MemberCode TEXT UNIQUE NOT NULL,
                                FullName TEXT NOT NULL,
                                Email TEXT,
                                Phone TEXT,
                                Address TEXT,
                                MemberShipDates DATETIME DEFAULT CURRENT_TIMESTAMP,
                                IsActive INTEGER DEFAULT 1
                            );";

                        SQLiteCommand fk = new SQLiteCommand("PRAGMA foreign_keys = ON;", con);

                        fk.ExecuteNonQuery();

                        SQLiteCommand createCmd = new SQLiteCommand(createtable, con);
                        createCmd.ExecuteNonQuery();

                        Console.WriteLine("Table Created.");



                        string insertquery = "INSERT INTO Members(MemberCode,FullName,Email,Phone,Address,MemberShipDates,IsActive)VALUES(@MemberCode,@FullName,@Email,@Phone,@Address,@MemberShipDates,@IsActive)";
                        SQLiteCommand cmd = new SQLiteCommand(insertquery, con);

                        cmd.Parameters.AddWithValue("@MemberCode", m.Membercode);
                        cmd.Parameters.AddWithValue("@FullName", m.FullName);
                        cmd.Parameters.AddWithValue("@Email", m.Email);
                        cmd.Parameters.AddWithValue("@Phone", m.Phone);
                        cmd.Parameters.AddWithValue("@Address", m.Address);
                        cmd.Parameters.AddWithValue("@MemberShipDates", m.Membershipdate);
                        cmd.Parameters.AddWithValue("@IsActive", m.IsActive);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            Console.WriteLine("Member Added Successfully");
                        }

                        con.Close();
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine("Database Error:" + ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }


            public void ViewAllMember()
            {
                Members m = new Members();
                Console.WriteLine("All Members");
                Console.WriteLine("________________________________________________________________________________________________________________________________________________\n");
                Console.WriteLine("MemberId\tMemberCode\tFullName Email\t\t Phone\t\tAddress\t\tMemberShipDates\t\tIsActive");

                try
                {
                    string cs = "Data Source=Library.db";
                    SQLiteConnection con = new SQLiteConnection(cs);

                    con.Open();

                    string selectsql = "SELECT * FROM Members";

                    SQLiteCommand cmd = new SQLiteCommand(selectsql, con);

                    SQLiteDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No Member found in Members table ");
                    }

                    while (reader.Read())
                    {
                        Console.WriteLine(
                            $"{reader["MemberId"]}  \t\t" +
                            $"{reader["MemberCode"]}  \t\t" +
                            $"{reader["FullName"]}  \t" +
                            $"{reader["Email"]}\t " +
                            $"{reader["Phone"]}  \t" +
                            $"{reader["Address"]}  \t" +
                            $"{reader["MemberShipDates"]}\t" +
                            $"{reader["IsActive"]}"

                        );

                    }
                    con.Close();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error" + ex.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error:" + e.Message);
                }

            }


            public void SearchMember()
            {
                Console.WriteLine("Enter MemberCode , Name or Phone to Search Member");
                string searchmember = Console.ReadLine();

                try
                {
                    string cs = "Data Source=Library.db";
                    SQLiteConnection con = new SQLiteConnection(cs);

                    con.Open();

                    string searchsql = @"SELECT * FROM Members where MemberCode LIKE @Search or FullName LIKE @Search or Phone LIKE @Search";

                    SQLiteCommand cmd = new SQLiteCommand(searchsql, con);

                    cmd.Parameters.AddWithValue("@Search", "%" + searchmember + "%");

                    SQLiteDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No Book Found");
                    }

                    Console.WriteLine("Available Member");
                    Console.WriteLine("________________________________________________________________________________________________________________________________________________\n");
                    Console.WriteLine("MemberCode\t Name\t Email\t Phone\t Address\t MemberShip Dates\t IsActive");

                    while (reader.Read())
                    {
                        Console.WriteLine(
                            $"{reader["MemberId"]}  \t\t" +
                            $"{reader["MemberCode"]}\t " +
                            $"{reader["FullName"]}\t " +
                            $"{reader["Email"]}\t " +
                            $"{reader["Phone"]}\t " +
                            $"{reader["Address"]}\t " +
                            $"{reader["MemberShipDates"]}\t " +
                            $"{reader["IsActive"]}"

                        );

                    }
                    con.Close();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error:" + ex.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error:" + e.Message);
                }
            }

            public void UpdateMembers()
            {
                Members m = new Members();
                Console.WriteLine("Enter MemberId to Update Member ");
                int memberid = Convert.ToInt32(Console.ReadLine());

                try
                {
                    string cs = "Data Source=Library.db";
                    SQLiteConnection con = new SQLiteConnection(cs);

                    con.Open();

                    int option;

                    Console.WriteLine("\n------ Update Menu ------");
                    Console.WriteLine("1. MemberCode");
                    Console.WriteLine("2. FullName");
                    Console.WriteLine("3. Email");
                    Console.WriteLine("4. Phone");
                    Console.WriteLine("5. Address");
                    Console.WriteLine("6. MemberShipDate");
                    Console.WriteLine("7. IsActive");

                    Console.Write("Enter Choice: ");
                    option = Convert.ToInt32(Console.ReadLine());

                    SQLiteCommand scmd = null;

                    switch (option)
                    {
                        case 1:
                            Console.WriteLine("Enter new MemberCode :");
                            m.Membercode = Console.ReadLine();

                            string updatemembercode = @"UPDATE Members SET MemberCode = @MemberCode Where MemberId = @MemberId";

                            scmd = new SQLiteCommand(updatemembercode, con);

                            scmd.Parameters.AddWithValue("@MemberCode", m.Membercode);
                            scmd.Parameters.AddWithValue("@MemberId", memberid);
                            break;

                        case 2:
                            Console.WriteLine("Enter new Name :");
                            m.FullName = Console.ReadLine();

                            string updatename = @"UPDATE Members SET FullName = @FullName Where MemberId = @MemberId";

                            scmd = new SQLiteCommand(updatename, con);

                            scmd.Parameters.AddWithValue("@FullName", m.FullName);
                            scmd.Parameters.AddWithValue("@MemberId", memberid);
                            break;

                        case 3:

                            while (true)
                            {
                                Console.WriteLine("Enter new Email :");
                                m.FullName = Console.ReadLine();

                                if (string.IsNullOrWhiteSpace(m.Email))
                                {
                                    break;
                                }

                                if (!Regex.IsMatch(
                                        m.Email,
                                        @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                                {
                                    Console.WriteLine("Invalid Email Format.");
                                    continue;
                                }
                                break;
                            }

                            string updateemail = @"UPDATE Members SET Email = @Email Where MemberId = @MemberId";

                            scmd = new SQLiteCommand(updateemail, con);

                            scmd.Parameters.AddWithValue("@Email", m.Email);
                            scmd.Parameters.AddWithValue("@MemberId", memberid);
                            break;

                        case 4:
                            while (true)
                            {
                                Console.Write("Enter Phone Number: ");
                                m.Phone = Console.ReadLine();

                                if (Regex.IsMatch(m.Phone, @"^\d{10}$"))
                                    break;

                                Console.WriteLine("Phone must be 10 digits.");
                            }


                            string updatephone = @"UPDATE Members SET Phone = @Phone Where MemberId = @MemberId";

                            scmd = new SQLiteCommand(updatephone, con);

                            scmd.Parameters.AddWithValue("@Phone", m.Phone);
                            scmd.Parameters.AddWithValue("@MemberId", memberid);
                            break;

                        case 5:
                            Console.WriteLine("Enter new Address :");
                            m.Address = Console.ReadLine();

                            string updateaddress = @"UPDATE Members SET Address = @Address Where MemberId = @MemberId";

                            scmd = new SQLiteCommand(updateaddress, con);

                            scmd.Parameters.AddWithValue("@Address", m.Address);
                            scmd.Parameters.AddWithValue("@MemberId", memberid);
                            break;

                        case 6:
                            while (true)
                            {
                                Console.WriteLine("Enter MemberShip Dates");
                                if (DateTime.TryParse(Console.ReadLine(), out DateTime membershipDate))
                                {
                                    m.Membershipdate = membershipDate;
                                    break;
                                }

                                Console.WriteLine("Invalid Formate");
                            }

                            string updatemembershipdate = @"UPDATE Members SET MemberShipDates = @MemberShipDates Where MemberId = @MemberId";

                            scmd = new SQLiteCommand(updatemembershipdate, con);

                            scmd.Parameters.AddWithValue("@Membershipdate", m.Membershipdate);
                            scmd.Parameters.AddWithValue("@MemberId", memberid);
                            break;

                        case 7:
                            while (true)
                            {
                                Console.Write("Enter IsActive 0 or 1: ");

                                if (int.TryParse(Console.ReadLine(), out int active)
                                    && (active == 0 || active == 1))
                                {
                                    m.IsActive = active;
                                    break;
                                }

                                Console.WriteLine("Enter only 0 or 1");
                            }
                            string updateisactive = @"UPDATE Members SET IsActive = @IsActive Where MemberId = @MemberId";

                            scmd = new SQLiteCommand(updateisactive, con);

                            scmd.Parameters.AddWithValue("@IsActive", m.IsActive);
                            scmd.Parameters.AddWithValue("@MemberId", memberid);
                            break;


                        default:
                            Console.WriteLine("Invalid Choice");
                            break;
                    }


                    int rows = scmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        Console.WriteLine("Member Updated Successfully");
                    }
                    else
                    {
                        Console.WriteLine("Member Not found");
                    }
                    con.Close();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error:" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }


            public void DeactivateMember()
            {
                int memberId;

                try
                {

                    while (true)
                    {
                        Console.Write("Enter Member Id: ");

                        if (int.TryParse(Console.ReadLine(), out memberId)
                            && memberId > 0)
                            break;

                        Console.WriteLine("Invalid Member Id.");
                    }

                    string cs = "Data Source=Library.db";
                    SQLiteConnection con = new SQLiteConnection(cs);

                    con.Open();

                    string query = @"UPDATE Members SET IsActive = 0 WHERE MemberId = @MemberId";

                    SQLiteCommand cmd = new SQLiteCommand(query, con);

                    cmd.Parameters.AddWithValue("@MemberId", memberId);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        Console.WriteLine("Member deactivated successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Member not found.");
                    }
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }

            public void ViewMemberBorrowingHistory()
            {
                Books b = new Books();
                Members m = new Members();
                BorrowedRecord br = new BorrowedRecord();

                Console.WriteLine("All View Member Borrowing History");
                Console.WriteLine("________________________________________________________________________________________________________________________________________________\n");
                Console.WriteLine("RecordId\tBookId\tMemberId\tBorrow Date\tDue Date\t\tReturn Date\t\tFine");

                try
                {
                    string cs = "Data Source=Library.db";
                    SQLiteConnection con = new SQLiteConnection(cs);

                    con.Open();

                    string selectsql = "SELECT * FROM BorrowRecords";

                    SQLiteCommand cmd = new SQLiteCommand(selectsql, con);

                    SQLiteDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No Borrow Record Found");
                    }

                    while (reader.Read())
                    {
                        Console.WriteLine(
                            $"{reader["RecordId"]}\t\t" +
                            $"{reader["BookId"]}\t" +
                            $"{reader["MemberId"]}\t" +
                            $"{reader["BorrowDate"]}\t" +
                            $"{reader["DueDate"]}\t" +
                            $"{reader["ReturnDate"]}\t" +
                            $"{reader["Fine"]}\t"

                        );

                    }
                    con.Close();

                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }


            public void IssueBook()
            {
                try
                {
                    string cs = "Data Source=Library.db";
                    SQLiteConnection con = new SQLiteConnection(cs);

                    con.Open();

                    while (true)
                    {
                        Console.Write("Enter MemberId: ");

                        if (int.TryParse(Console.ReadLine(), out int MemberId) && MemberId > 0)
                        {
                            br.MemberId = MemberId;
                            break;
                        }

                        Console.WriteLine("Invalid MemberId");
                    }

                    string checkMember = @"Select IsActive FROM Members Where MemberId = @MemberId";

                    SQLiteCommand cmds = new SQLiteCommand(checkMember, con);
                    cmds.Parameters.AddWithValue("@MemberId", br.MemberId);

                    int memberExists = Convert.ToInt32(cmds.ExecuteScalar());

                    if (memberExists == 0)
                    {
                        Console.WriteLine("Member not found.");

                    }


                    int isActive = Convert.ToInt32(memberExists);

                    if (isActive != 1)
                    {
                        Console.WriteLine("Member is inactive. Cannot issue book.");
                        return;
                    }

                    Console.WriteLine("Member is active. Book can be issued.");


                    while (true)
                    {
                        Console.Write("Enter BookId: ");

                        if (int.TryParse(Console.ReadLine(), out int BookId) && BookId > 0)
                        {
                            br.BookId = BookId;
                            break;
                        }

                        Console.WriteLine("Invalid BookId.");
                    }


                    string bookexits = @"SELECT AvailableCopies FROM Books Where BookId = @BookId";

                    SQLiteCommand cmd = new SQLiteCommand(bookexits, con);

                    cmd.Parameters.AddWithValue("@BookId", br.BookId);

                    object result = cmd.ExecuteScalar();

                    if (result == null)
                    {
                        Console.WriteLine("Books not found");
                    }

                    int availableCopies = Convert.ToInt32(result);

                    if (availableCopies <= 0)
                    {
                        Console.WriteLine("Book is  unavailable.");
                        return;
                    }

                    br.BorrowDate = DateTime.Now;

                    br.ReturnDate = null;


                    Console.WriteLine("Database Connection Successfully");

                    string createtable = @"

                            CREATE TABLE IF NOT EXISTS BorrowRecords
                            (
                                RecordId INTEGER PRIMARY KEY AUTOINCREMENT,
                                BookId INTEGER, 
                                MemberId INTEGER, 
                                BorrowDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                                DueDate DATETIME NOT NULL,
                                ReturnDate DATETIME NULL,
                                Fine DECIMAL DEFAULT 0,
                                FOREIGN KEY(BookId) REFERENCES Books (BookID),
                                FOREIGN KEY(MemberId) REFERENCES Members (MemberId)
                            )";

                    SQLiteCommand fk = new SQLiteCommand("PRAGMA foreign_keys = ON;", con);

                    fk.ExecuteNonQuery();

                    SQLiteCommand createCmd = new SQLiteCommand(createtable, con);
                    createCmd.ExecuteNonQuery();

                    Console.WriteLine("Table Created.");


                    string bookissue = @"INSERT INTO BorrowRecords(BookId,MemberId,BorrowDate,DueDate)VALUES(@BookId,@MemberId,DATETIME('now'), DATETIME('now', '+14 days'))";

                    SQLiteCommand insertcmd = new SQLiteCommand(bookissue, con);

                    insertcmd.Parameters.AddWithValue("@BookId", br.BookId);
                    insertcmd.Parameters.AddWithValue("@MemberId", br.MemberId);

                    insertcmd.ExecuteNonQuery();

                    string updateBookQuery = @"UPDATE Books SET AvailableCopies = AvailableCopies - 1 WHERE BookId = @BookId";

                    SQLiteCommand updateCmd = new SQLiteCommand(updateBookQuery, con);

                    updateCmd.Parameters.AddWithValue("@BookId", br.BookId);

                    updateCmd.ExecuteNonQuery();

                    con.Close();

                    Console.WriteLine("Book issued successfully.");
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }



            public void DisplayAllIssueBooks()
            {
                Books b = new Books();
                Members m = new Members();
                BorrowedRecord br = new BorrowedRecord();

                Console.WriteLine("All Issue Books");
                Console.WriteLine("________________________________________________________________________________________________________________________________________________\n");
                Console.WriteLine("RecordId\tBookId\tMemberId\tBorrow Date\tDue Date\t\tReturn Date\t\tFine");

                try
                {
                    string cs = "Data Source=Library.db";
                    SQLiteConnection con = new SQLiteConnection(cs);

                    con.Open();

                    string selectsql = "SELECT * FROM BorrowRecords";

                    SQLiteCommand cmd = new SQLiteCommand(selectsql, con);

                    SQLiteDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No Borrow Record Found");
                    }

                    while (reader.Read())
                    {
                        Console.WriteLine(
                            $"{reader["RecordId"]}\t\t" +
                            $"{reader["BookId"]}\t" +
                            $"{reader["MemberId"]}\t" +
                            $"{reader["BorrowDate"]}\t" +
                            $"{reader["DueDate"]}\t" +
                            $"{reader["ReturnDate"]}\t" +
                            $"{reader["Fine"]}\t"

                        );

                    }
                    con.Close();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }


            public void ReturnBook()
            {
                int bookId;
                int memberId;

                try
                {

                    while (true)
                    {
                        Console.Write("Enter Book Id: ");

                        if (int.TryParse(Console.ReadLine(), out bookId) && bookId > 0)
                            break;

                        Console.WriteLine("Invalid Book Id.");
                    }

                    while (true)
                    {
                        Console.Write("Enter Member Id: ");

                        if (int.TryParse(Console.ReadLine(), out memberId) && memberId > 0)
                            break;

                        Console.WriteLine("Invalid Member Id.");
                    }

                    string cs = "Data Source=Library.db";

                    SQLiteConnection con = new SQLiteConnection(cs);

                    con.Open();

                    string checkBorrow = @"SELECT RecordId,DueDate FROM BorrowRecords WHERE BookId = @BookId AND MemberId = @MemberId AND ReturnDate IS NULL";

                    SQLiteCommand checkCmd = new SQLiteCommand(checkBorrow, con);

                    checkCmd.Parameters.AddWithValue("@BookId", bookId);
                    checkCmd.Parameters.AddWithValue("@MemberId", memberId);

                    SQLiteDataReader reader = checkCmd.ExecuteReader();

                    if (!reader.Read())
                    {
                        Console.WriteLine("No active borrowed book found.");
                        reader.Close();
                        return;
                    }

                    int recordId = Convert.ToInt32(reader["RecordId"]);
                    DateTime dueDate = Convert.ToDateTime(reader["DueDate"]);

                    reader.Close();




                    DateTime returnDate;

                    while (true)
                    {
                        Console.Write("Enter Return Date (dd-MM-yyyy): ");

                        if (DateTime.TryParse(Console.ReadLine(), out returnDate))
                        {
                            break;
                        }

                        Console.WriteLine("Invalid Date Format.");
                    }

                    int overdueDays = Math.Max(0, (returnDate.Date - dueDate.Date).Days);

                    decimal fine = overdueDays;

                    fine = Math.Min(fine, 50);


                    string updateBorrow = @"UPDATE BorrowRecords SET ReturnDate = @ReturnDate,Fine = @Fine WHERE RecordId = @RecordId";

                    SQLiteCommand borrowCmd = new SQLiteCommand(updateBorrow, con);

                    borrowCmd.Parameters.AddWithValue("@ReturnDate", returnDate);
                    borrowCmd.Parameters.AddWithValue("@Fine", fine);
                    borrowCmd.Parameters.AddWithValue("@RecordId", recordId);

                    borrowCmd.ExecuteNonQuery();

                    string updateBook = @"UPDATE Books SET AvailableCopies = AvailableCopies + 1 WHERE BookId = @BookId";

                    SQLiteCommand bookCmd = new SQLiteCommand(updateBook, con);

                    bookCmd.Parameters.AddWithValue("@BookId", bookId);

                    bookCmd.ExecuteNonQuery();

                    Console.WriteLine("\nBook Returned Successfully");
                    Console.WriteLine($"Return Date : {returnDate:dd-MM-yyyy HH:mm:ss}");
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }





            private Books ReportsAnalyticsOfBooks()
            {


                Books b = new Books();
                string cs = "Data Source=Library.db";

                SQLiteConnection con = new SQLiteConnection(cs);

                con.Open();

                SQLiteCommand cmd;

                string bookquery = @"SELECT COUNT(*) FROM Books";
                cmd = new SQLiteCommand(bookquery, con);
                b.totalBooks = Convert.ToInt32(cmd.ExecuteScalar());

                string Membersquery = @"SELECT COUNT(*) FROM Members WHERE IsActive = 1";
                cmd = new SQLiteCommand(Membersquery, con);
                b.totalMember = Convert.ToInt32(cmd.ExecuteScalar());

                string borrowquery = @"SELECT COUNT(*) FROM BorrowRecords WHERE ReturnDate IS NULL";
                cmd = new SQLiteCommand(borrowquery, con);
                b.borrowbook = Convert.ToInt32(cmd.ExecuteScalar());

                string overduequery = @"SELECT COUNT(*) FROM BorrowRecords WHERE ReturnDate IS NOT NULL AND DATE(ReturnDate) > DATE(DueDate)";
                cmd = new SQLiteCommand(overduequery, con);
                b.overduebook = Convert.ToInt32(cmd.ExecuteScalar());

                string availablecopy = @"SELECT IFNULL(SUM(AvailableCopies),0)FROM Books";
                cmd = new SQLiteCommand(availablecopy, con);
                b.AvailableCopies = Convert.ToInt32(cmd.ExecuteScalar());

                string borrowcopy = @"SELECT IFNULL(SUM(TotalCopies - AvailableCopies),0)FROM Books";
                cmd = new SQLiteCommand(borrowcopy, con);
                b.borrowedCopies = Convert.ToInt32(cmd.ExecuteScalar());

                return b;
            }

            public void DisplayDashboard()
            {

                Books report = ReportsAnalyticsOfBooks();
                Console.WriteLine(
                    $"Total Books: {report.totalBooks} | " +
                    $"Total Members: {report.totalMember} | " +
                    $"Total Books Issue: {report.borrowbook} | " +
                    $"Total Overdue Book: {report.overduebook} | " +
                    $"Available Copies: {report.AvailableCopies} | " +
                    $"Borrowed Copies: {report.borrowedCopies}");

                Console.WriteLine();

            }



            public void TotalBooks()
            {
                try
                {
                    SQLiteConnection con = new SQLiteConnection("Data Source=Library.db");

                    con.Open();

                    string query = "SELECT COUNT(*) FROM Books";

                    SQLiteCommand cmd =
                        new SQLiteCommand(query, con);

                    int count =
                        Convert.ToInt32(cmd.ExecuteScalar());

                    Console.WriteLine($"\nTotal Books : {count}");
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }





            public void TotalMembers()
            {
                try
                {
                    SQLiteConnection con = new SQLiteConnection("Data Source=Library.db");

                    con.Open();

                    string query =
                        "SELECT COUNT(*) FROM Members WHERE IsActive = 1";

                    SQLiteCommand cmd =
                        new SQLiteCommand(query, con);

                    int count =
                        Convert.ToInt32(cmd.ExecuteScalar());

                    Console.WriteLine($"\nTotal Members : {count}");
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }

            public void BorrowedBooksCount()
            {
                try
                {
                    SQLiteConnection con = new SQLiteConnection("Data Source=Library.db");

                    con.Open();

                    string query = @"SELECT COUNT(*) FROM BorrowRecords WHERE ReturnDate IS NULL";

                    SQLiteCommand cmd = new SQLiteCommand(query, con);

                    int count =
                        Convert.ToInt32(cmd.ExecuteScalar());

                    Console.WriteLine($"\nBooks Out : {count}");
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }

            public void OverdueBooks()
            {
                try
                {
                    SQLiteConnection con = new SQLiteConnection("Data Source=Library.db");

                    con.Open();

                    string query = @"SELECT BookId,MemberId,DueDate,Fine FROM BorrowRecords WHERE ReturnDate IS  NULL AND DATE(DueDate) > DATE('now')";

                    SQLiteCommand cmd =
                        new SQLiteCommand(query, con);

                    SQLiteDataReader reader =
                        cmd.ExecuteReader();

                    Console.WriteLine("\nBookId\tMemberId\tDueDate\t\tFine");

                    while (reader.Read())
                    {
                        Console.WriteLine(
                            $"{reader["BookId"]}\t" +
                            $"{reader["MemberId"]}\t\t" +
                            $"{reader["DueDate"]}\t" +
                            $"{reader["Fine"]}");
                    }
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }

            public void TopBorrowedBooks()
            {
                try
                {
                    SQLiteConnection con = new SQLiteConnection("Data Source=Library.db");

                    con.Open();

                    string query = @"SELECT BookId, COUNT(*) AS TotalBorrowed FROM BorrowRecords GROUP BY BookId ORDER BY TotalBorrowed DESC LIMIT 10";

                    SQLiteCommand cmd = new SQLiteCommand(query, con);

                    SQLiteDataReader reader = cmd.ExecuteReader();

                    Console.WriteLine("\nBookId\tTotal Borrowed");

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["BookId"]}\t{reader["TotalBorrowed"]}");
                    }
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }


            public void MostActiveMembers()
            {
                try
                {
                    SQLiteConnection con = new SQLiteConnection("Data Source=Library.db");

                    con.Open();

                    string query = @"SELECT MemberId,COUNT(*) AS BorrowCount FROM BorrowRecords GROUP BY MemberId ORDER BY BorrowCount DESC  LIMIT 10";

                    SQLiteCommand cmd = new SQLiteCommand(query, con);

                    SQLiteDataReader reader = cmd.ExecuteReader();

                    Console.WriteLine("\nMemberId\tBorrow Count");

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["MemberId"]}\t{reader["BorrowCount"]}");
                    }
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }

            public void BooksNeverBorrowed()
            {
                try
                {
                    SQLiteConnection con = new SQLiteConnection("Data Source=Library.db");

                    con.Open();

                    string query = @"SELECT BookId, Title FROM Books WHERE BookId NOT IN( SELECT DISTINCT BookId FROM BorrowRecords)";

                    SQLiteCommand cmd = new SQLiteCommand(query, con);

                    SQLiteDataReader reader = cmd.ExecuteReader();

                    Console.WriteLine("\nBookId\tTitle");

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["BookId"]}\t{reader["Title"]}");
                    }
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }


            public void CategoryDistribution()
            {
                try
                {
                    SQLiteConnection con = new SQLiteConnection("Data Source=Library.db");

                    con.Open();

                    string query = @"SELECT Category, COUNT(*) AS TotalBooks FROM Books GROUP BY Category";

                    SQLiteCommand cmd = new SQLiteCommand(query, con);

                    SQLiteDataReader reader = cmd.ExecuteReader();

                    Console.WriteLine("\nCategory\tTotal Books");

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Category"]}\t{reader["TotalBooks"]}");
                    }
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }

            public void MonthlyBorrowStats()
            {
                try
                {
                    SQLiteConnection con = new SQLiteConnection("Data Source=Library.db");

                    con.Open();

                    string query = @"SELECT strftime('%Y-%m', BorrowDate) AS Month,COUNT(*) AS TotalBorrowed FROM BorrowRecords GROUP BY Month ORDER BY Month";

                    SQLiteCommand cmd = new SQLiteCommand(query, con);

                    SQLiteDataReader reader = cmd.ExecuteReader();

                    Console.WriteLine("\nMonth\t\tBorrow Count");

                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Month"]}\t\t{reader["TotalBorrowed"]}");
                    }
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("Database Error" + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }

        }



        static void Main()
        {
            Library lb = new Library();
            Books b = new Books();
            int mainchoice;

            try
            {
                Console.Clear();


                do
                {
                    Console.WriteLine("====Library Management System====\n");

                    Console.WriteLine("Main Menu\n");
                    Console.WriteLine("1.Book Management: ");
                    Console.WriteLine("2.Member Management: ");
                    Console.WriteLine("3.Borrow / Return Books: ");
                    Console.WriteLine("4.Reports & Statistics: ");
                    Console.WriteLine("5.Database Backup: ");
                    Console.WriteLine("6.Exit:");

                    Console.WriteLine("Enter Your Choice");
                    mainchoice = Convert.ToInt32(Console.ReadLine());


                    switch (mainchoice)
                    {
                        case 1:
                            BookManagementMenu();
                            break;

                        case 2:
                            MemberManagementmenu();
                            break;

                        case 3:
                            BorrowandReturnSystem();
                            break;

                        case 4:
                            ReportsMenu();
                            break;

                        case 5:
                            BackupDatabase();
                            break;

                        case 6:
                            Console.WriteLine("Exit...");
                            break;

                        case 7:
                        default:
                            Console.WriteLine("Invalid choice");
                            break;
                    }
                } while (mainchoice != 7);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }

        public static void BookManagementMenu()
        {
            Library lb = new Library();
            int choice;
            try
            {
                do
                {
                    Console.Clear();

                    Console.WriteLine(" ------ BOOK MANAGEMENT MENU ------");
                    Console.WriteLine("1. Add New Book");
                    Console.WriteLine("2. View All Books");
                    Console.WriteLine("3. Search Books");
                    Console.WriteLine("4. Update Book");
                    Console.WriteLine("5. Delete Book");
                    Console.WriteLine("6. Back to main Menu");
                    Console.WriteLine("7. Exit..");

                    Console.Write("Enter choice: ");
                    int.TryParse(Console.ReadLine(), out choice);

                    switch (choice)
                    {
                        case 1:
                            lb.AddNewBook();
                            break;

                        case 2:
                            lb.ViewAllBooks();
                            lb.DisplayDashboard();
                            break;

                        case 3:
                            lb.SearchOperation();
                            break;

                        case 4:
                            lb.UpdateBook();
                            break;

                        case 5:
                            lb.DeleteBooks();
                            break;

                        case 6:
                            return;

                        case 7:
                            Console.WriteLine("Exit..");
                            break;

                        default:
                            Console.WriteLine("Invalid Choice!");
                            break;
                    }

                    Console.ReadKey();

                } while (choice != 8);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }


        public static void MemberManagementmenu()
        {
            Library lb = new Library();
            int choice;


            try
            {
                do
                {
                    Console.Clear();

                    Console.WriteLine("----- MemberManagement Menu -----");
                    Console.WriteLine("1. Add New Member");
                    Console.WriteLine("2. View All Member");
                    Console.WriteLine("3. Search Member Details");
                    Console.WriteLine("4. Update Member Details");
                    Console.WriteLine("5. Deactivate members ");
                    Console.WriteLine("6. View member borrowing history ");
                    Console.WriteLine("7. Back to Main Menu");

                    Console.Write("Enter choice: ");
                    int.TryParse(Console.ReadLine(), out choice);

                    switch (choice)
                    {
                        case 1:
                            lb.AddMembers();
                            break;
                        case 2:
                            lb.ViewAllMember();
                            lb.DisplayDashboard();
                            break;
                        case 3:
                            lb.SearchMember();
                            break;
                        case 4:
                            lb.UpdateMembers();
                            break;
                        case 5:
                            lb.DeactivateMember();
                            break;
                        case 6:
                            lb.ViewMemberBorrowingHistory();
                            break;
                        case 7:
                            return;
                        default:
                            Console.WriteLine("Invalid Choice");
                            break;

                    }

                    Console.ReadKey();

                } while (choice != 8);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }


        public static void BorrowandReturnSystem()
        {
            Library lb = new Library();
            int choice;

            try
            {
                do
                {
                    Console.Clear();

                    Console.WriteLine("----- Borrow And Return System -----");
                    Console.WriteLine("1. Issue Book to Member");
                    Console.WriteLine("2. Return Books to Library");
                    Console.WriteLine("3. View Borrow Books Record ");
                    Console.WriteLine("4. Exit");
                    Console.WriteLine("5. Back to Main Menu");

                    Console.Write("Enter choice: ");
                    int.TryParse(Console.ReadLine(), out choice);

                    switch (choice)
                    {
                        case 1:
                            lb.IssueBook();
                            break;
                        case 2:
                            lb.ReturnBook();
                            break;
                        case 3:
                            lb.DisplayAllIssueBooks();
                            break;
                        case 4:
                            Console.WriteLine("Exit..");
                            break;
                        case 5:
                            return;

                        default:
                            Console.WriteLine("Invalid Choice");
                            break;

                    }

                    Console.ReadKey();

                } while (choice != 6);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }

        public static void BackupDatabase()
        {
            try
            {
                string sourceFile = "Library.db";

                string backupFolder = "D:\\meet\\BackupLibrarydb";

                if (!Directory.Exists(backupFolder))
                {
                    Directory.CreateDirectory(backupFolder);
                }

                string backupFile = Path.Combine(backupFolder, $"Library_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");

                File.Copy(sourceFile, backupFile, true);

                Console.WriteLine("\nDatabase Backup Successful.");
                Console.WriteLine($"Backup File: {backupFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }


        public static void ReportsMenu()
        {
            try
            {
                Library lb = new Library();
                while (true)
                {
                    Console.Clear();

                    Console.WriteLine("===== Reports & Analytics =====");
                    Console.WriteLine("1. Total Books in Library");
                    Console.WriteLine("2. Total Registered Members");
                    Console.WriteLine("3. Currently Borrowed Books");
                    Console.WriteLine("4. Overdue Books List");
                    Console.WriteLine("5. Top 10 Most Borrowed Books");
                    Console.WriteLine("6. Most Active Members");
                    Console.WriteLine("7. Books Never Borrowed");
                    Console.WriteLine("8. Category Wise Distribution");
                    Console.WriteLine("9. Monthly Borrow Statistics");
                    Console.WriteLine("10. Back");

                    Console.Write("\nEnter Choice: ");

                    int choice = Convert.ToInt32(Console.ReadLine());

                    switch (choice)
                    {
                        case 1:
                            lb.TotalBooks();
                            break;

                        case 2:
                            lb.TotalMembers();
                            break;

                        case 3:
                            lb.BorrowedBooksCount();
                            break;

                        case 4:
                            lb.OverdueBooks();
                            break;

                        case 5:
                            lb.TopBorrowedBooks();
                            break;

                        case 6:
                            lb.MostActiveMembers();
                            break;

                        case 7:
                            lb.BooksNeverBorrowed();
                            break;

                        case 8:
                            lb.CategoryDistribution();
                            break;

                        case 9:
                            lb.MonthlyBorrowStats();
                            break;

                        case 10:
                            return;
                    }

                    Console.WriteLine("\nPress Any Key...");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }
    }
}
