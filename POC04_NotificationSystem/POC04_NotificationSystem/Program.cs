using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace POC04_NotificationSystem
{
    internal class Program
    {
        interface INotificationService
        {
            void SendNotification();
            void GetServiceName();
        }

        class Notification : INotificationService
        {
            public List<string> notification = new List<string>();
            public string message { get; set; }
            public string recipient { get; set; }

            public string fmessage { get; set; }
            public string frecipient { get; set; }

            public string emessage { get; set; }
            public string erecipient { get; set; }

            public string esubject { get; set; }

            public int NotificationCount = 0;


            public DateTime Date { get; set; } = DateTime.Now;

            public void SendNotification()
            {
                

                bool valid = false;
                try
                {
                    while (!valid)
                    {
                        Console.WriteLine("Enter Recipient:");
                        recipient = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(recipient))
                        {
                            Console.WriteLine("Recipient cannot be empty.");
                            continue;
                        }

                        valid = true;

                        foreach (Char ch in recipient)
                        {
                            if (!char.IsLetterOrDigit(ch) && ch != ' ')
                            {
                                Console.WriteLine("Only Contain Letters and Space.");
                                valid = false;
                                break;
                            }
                        }


                        Console.WriteLine("Enter Message:");
                        message = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(message))
                        {
                            Console.WriteLine("Message cannot be empty.");
                            continue;
                        }

                        valid = true;

                        foreach (char msg in message)
                        {
                            if (!char.IsLetterOrDigit(msg) && msg != ' ')
                            {
                                Console.WriteLine("Only Contain Letters and Space.");
                                valid = false;
                                break;
                            }
                        }

                        NotificationCount++;
                        Console.WriteLine(" Console Notification Sent Successfully!");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Sending Notification" + ex.Message);
                }

                
            }


            public void EmailNotification()
            {

                try
                {
                    bool valid = false;
                    while (!valid)
                    {
                        Console.WriteLine("Enter Recipient:");
                        erecipient = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(erecipient))

                        {
                            Console.WriteLine("Recipient cannot be empty.");
                            continue;
                        }

                        valid = true;

                        foreach (Char ch in erecipient)
                        {
                            if (!char.IsLetterOrDigit(ch) && ch != ' ')
                            {
                                Console.WriteLine("Only Contain Letters and Space.");
                                valid = false;
                                break;
                            }
                        }

                        valid = true;

                        Console.WriteLine("Enter Subject:");
                        esubject = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(esubject))
                        {
                            Console.WriteLine("Subject cannot be empty.");
                            continue;
                        }

                        foreach (char sub in esubject)
                        {
                            if (!char.IsLetterOrDigit(sub) && sub != ' ')
                            {
                                Console.WriteLine("Only Contain Letters, Space and Digits.");
                                valid = false;
                                break;
                            }
                        }

                        valid = true;

                        Console.WriteLine("Enter Message:");
                        emessage = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(emessage))
                        {
                            Console.WriteLine("Message cannot be empty.");
                            continue;
                        }

                        valid = true;

                        foreach (char ch in emessage)
                        {
                            if (!char.IsLetter(ch) && ch != ' ')
                            {
                                Console.WriteLine("Only Contain Letters and Space.");
                                valid = false;
                                break;
                            }
                        }

                        NotificationCount++;
                        Console.WriteLine("Email Notification Sent Successfully!");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Sending Notification" + ex.Message);
                }

               
            }

            public void FileNotification()
            {

                try
                {
                    try
                    {
                        bool valid = false;
                        while (!valid)
                        {
                            Console.WriteLine("Enter Recipient:");
                            frecipient = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(frecipient))
                            {
                                Console.WriteLine("Recipient cannot be empty.");
                                continue;
                            }

                            valid = true;

                            foreach (Char ch in frecipient)
                            {
                                if (!char.IsLetter(ch) && ch != ' ')
                                {
                                    Console.WriteLine("Only Contain Letters and Space.");
                                    valid = false;
                                    break;
                                }
                            }

                            if (!valid)
                            {
                                continue;
                            }

                            Console.WriteLine("Enter Message:");
                            fmessage = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(fmessage))
                            {
                                Console.WriteLine("Message cannot be empty.");
                                continue;
                            }

                            foreach (char msg in fmessage)
                            {
                                if (!char.IsLetter(msg) && msg != ' ')
                                {
                                    Console.WriteLine("Only Contain Letters and Space.");
                                    valid = false;
                                    break;
                                }
                            }

                            NotificationCount++;
                            Console.WriteLine("File Notification Sent Successfully!");

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: Sending Notification" + ex.Message);
                    }

                   

                    string FilePath = "D:\\meet\\FileNotification.txt";
                    using (StreamWriter sw = new StreamWriter(FilePath, true))
                    {
                        sw.WriteLine("========================================================\n");
                        sw.WriteLine("File Notification\n");
                        sw.WriteLine("========================================================\n");

                        sw.WriteLine($"To: {frecipient}");
                        sw.WriteLine($"Time: {Date}");
                        sw.WriteLine($"Message : {fmessage}\n");

                        sw.WriteLine("========================================================\n");

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Writing to file" + ex.Message);
                }
            }

            public void GetServiceName()
            {

                if (string.IsNullOrWhiteSpace(recipient) ||
                    string.IsNullOrWhiteSpace(message))

                {
                    Console.WriteLine("Console Notification Not Found.");
                }
                else
                {
                    DisplayConsoleNotification();
                    void DisplayConsoleNotification()
                    {
                        Console.WriteLine("========================================================\n");
                        Console.WriteLine("Console Notification\n");
                        Console.WriteLine("========================================================\n");

                        Console.WriteLine($"To: {recipient}");
                        Console.WriteLine($"Time: {Date}");
                        Console.WriteLine($"Message : {message}\n");

                        Console.WriteLine("========================================================\n");

                    }
                }


                if (string.IsNullOrWhiteSpace(erecipient) ||
                    string.IsNullOrWhiteSpace(emessage) ||
                    string.IsNullOrWhiteSpace(esubject))
                {
                    Console.WriteLine("Email Notification Not Found");
                }
                else
                {
                    DisplayEmailNotification();
                    void DisplayEmailNotification()
                    {
                        Console.WriteLine("========================================================\n");
                        Console.WriteLine("Email Notification\n");
                        Console.WriteLine("========================================================\n");
                        Console.WriteLine($"To: {erecipient}");
                        Console.WriteLine($"Subject: {esubject}");
                        Console.WriteLine($"Time: {Date}");
                        Console.WriteLine($"Message : {emessage}\n");
                        Console.WriteLine("========================================================\n");

                    }

                    DisplayFileNotification();
                    void DisplayFileNotification()
                    {
                        string FilePath = "D:\\meet\\FileNotification.txt";
                        if (File.Exists(FilePath))
                        {
                            Console.WriteLine("\n===== Notification History =====\n");
                            Console.WriteLine(File.ReadAllText(FilePath));
                        }
                        else
                        {
                            Console.WriteLine("File notification history Not found.");
                        }

                    }

                    Console.WriteLine($"Total Notifications Sent: {NotificationCount}\n");
                }
            }

            static void Main(string[] args)
            {
                Notification n = new Notification();
                void ChangeService()
                {
                    int subChoice;

                    try
                    {
                        while (true)
                        {

                            Console.WriteLine("===== Change Service =====\n");
                            Console.WriteLine("1. Console Notification");
                            Console.WriteLine("2. Email Notification");
                            Console.WriteLine("3. File Notification");
                            Console.WriteLine("4. Back to Main Menu");
                            Console.WriteLine("5. Exit");

                            Console.WriteLine("Enter Choice :");
                            subChoice = Convert.ToInt32(Console.ReadLine());

                            switch (subChoice)
                            {
                                case 1:
                                    n.SendNotification();
                                    break;
                                case 2:
                                    n.EmailNotification();
                                    break;
                                case 3:
                                    n.FileNotification();
                                    break;
                                case 4:
                                    Console.WriteLine("Returning to Main Menu...");
                                    return;
                                case 5:
                                    Console.WriteLine("Exiting...");
                                    break;
                                default:
                                    Console.WriteLine("Invalid");
                                    break;
                            }
                        }
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("Invalid input format. Please enter a valid number." + e.Message);
                    }
                }

                try
                {
                    int mainchoice;
                    while (true)
                    {
                        Console.WriteLine("===== Notification System =====");
                        Console.WriteLine("1. Send Notification");
                        Console.WriteLine("2. View History");
                        Console.WriteLine("3. Change Service");
                        Console.WriteLine("4. Exit");

                        Console.WriteLine("Enter Choice :");
                        mainchoice = Convert.ToInt32(Console.ReadLine());

                        switch (mainchoice)
                        {
                            case 1:
                                n.SendNotification();
                                break;
                            case 2:
                                n.GetServiceName();
                                break;
                            case 3:
                                ChangeService();
                                break;
                            case 4:
                                Console.WriteLine("Exiting...");
                                break;
                            default:
                                Console.WriteLine("Invalid");
                                break;
                        }
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Invalid input format. Please enter a valid number." + e.Message);
                }
            }
        }
    }
}

















