using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace POC05_JsonFileOperation_ContactBook
{
    internal class Program
    {
        class Contact
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public string Category { get; set; }
            public DateTime date { get; set; } = DateTime.Now;
            public bool IsFavorite { get; set; }

        }

        class ContactBook
        {
            public List<Contact> Contacts { get; set; } = new List<Contact>();
            public DateTime LastModified { get; set; } = new DateTime();
            public int TotalContacts { get; set; }


            public void AddNewContact()
            {
                Contact c = new Contact();
                ContactBook cb = new ContactBook();

                try
                {

                    string FilePath = "D:\\meet\\Contacts.json";

                    string jsonData = File.ReadAllText(FilePath);

                    ContactBook contactBook = JsonConvert.DeserializeObject<ContactBook>(jsonData);

                    while (true)
                    {
                        Console.WriteLine("Enter Your Name :");
                        c.Name = Console.ReadLine();

                        if (String.IsNullOrWhiteSpace(c.Name))
                        {
                            Console.WriteLine("Name cannot be empty.");
                            continue;
                        }

                        bool validName = true;

                        foreach (char ch in c.Name)
                        {
                            if (!char.IsLetter(ch) && !char.IsWhiteSpace(ch))
                            {
                                Console.WriteLine("Name can only contain letters and spaces.");
                                validName = false;
                                break;

                            }

                        }

                        if (validName)
                            break;
                    }


                    while (true)
                    {
                        Console.WriteLine("Enter Your Phone Number :");
                        c.PhoneNumber = Convert.ToString(Console.ReadLine());

                        if (string.IsNullOrWhiteSpace(c.PhoneNumber))
                        {
                            Console.WriteLine("Phone number cannot be empty.");
                            continue;
                        }

                        bool phoneExists = contactBook.Contacts.Any(x => x.PhoneNumber == c.PhoneNumber);

                        if (phoneExists)
                        {
                            Console.WriteLine("Phone Number already exists.");
                            continue;
                        }

                        string pattern = @"^[6-9]\d{9}$";

                        if (Regex.IsMatch(c.PhoneNumber, pattern))
                        {
                            Console.WriteLine("Valid");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid PhoneNumber Format");
                        }
                        break;
                    }


                        while (true)
                        {
                            Console.WriteLine("Enter Email :");
                            c.Email = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(c.Email))
                            {
                                Console.WriteLine("Email cannot be empty.");
                                continue;
                            }

                            bool emailExists = contactBook.Contacts.Any(x => x.Email.Equals(c.Email, StringComparison.OrdinalIgnoreCase));

                            if (emailExists)
                            {
                                Console.WriteLine("Email already exists.");
                                continue;
                            }

                        string patternEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

                            if (Regex.IsMatch(c.Email, patternEmail))
                            {
                                Console.WriteLine("Valid Emaill");
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid Email Formate");
                            }

                           break;

                        }


                        while (true)
                        {
                            Console.WriteLine("Enter Your Address :");
                            c.Address = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(c.Address))
                            {
                                Console.WriteLine("Address cannot be empty.");
                                continue;
                            }

                            bool ValidAddress = true;

                            foreach (char ch in c.Address)
                            {
                                if (!char.IsLetterOrDigit(ch) && !char.IsWhiteSpace(ch) && ch != ',')
                                {
                                    Console.WriteLine("Address can only contain letters, digits, spaces");
                                    break;

                                }
                            }

                            if (ValidAddress)
                                break;
                        }

                        try
                        {
                            int choice;
                            do
                            {
                                Console.WriteLine("Select Category:");
                                Console.WriteLine("1. Family");
                                Console.WriteLine("2. Friend");
                                Console.WriteLine("3. Work");
                                Console.WriteLine("4. Other");

                                Console.WriteLine("Select a category:");
                                choice = Convert.ToInt32(Console.ReadLine());


                                switch (choice)
                                {
                                    case 1:
                                        c.Category = "Family";
                                        break;
                                    case 2:
                                        c.Category = "Friend";
                                        break;
                                    case 3:
                                        c.Category = "Work";
                                        break;
                                    case 4:
                                        c.Category = "Other";
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice. Please select a valid category.");
                                        continue;
                                }
                                break;

                            } while (true);
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine("Error: String Formating" + e.Message);
                        }



                        while (true)
                        {
                            Console.WriteLine("Mark as favorite? (y/n):");
                            string favoriteInput = Console.ReadLine().Trim().ToLower();

                            if (favoriteInput == "y")
                            {
                                c.IsFavorite = true;
                                break;
                            }
                            else if (favoriteInput == "n")
                            {
                                c.IsFavorite = false;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
                            }

                        }

                        contactBook.Contacts.Add(c);

                        contactBook.LastModified = DateTime.Now;
                        contactBook.TotalContacts = contactBook.Contacts.Count;

                    string Jsondata = JsonConvert.SerializeObject(contactBook, Newtonsoft.Json.Formatting.Indented);

                        File.WriteAllText(FilePath, Jsondata);
                        Console.WriteLine("Contact added successfully!");
                    }
                catch (FormatException e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }
            }


            public void SearchWithName()
            {

                try
                {
                    string FilePath = "D:\\meet\\Contacts.json";

                    string jsonData = File.ReadAllText(FilePath);

                    ContactBook contactBook = JsonConvert.DeserializeObject<ContactBook>(jsonData);

                    while (true)
                    {
                        Console.WriteLine("Enter Name to Search Contacts:");
                        string SearchName = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(SearchName))
                        {
                            Console.WriteLine("Name Cannot Empty or Null");
                            continue;
                        }

                        var results = contactBook.Contacts.Where(c =>
                        c.Name.Equals(SearchName, StringComparison.OrdinalIgnoreCase)).ToList();

                        if (results.Count > 0)
                        {
                            Console.WriteLine("Contact found\n");

                            foreach (var namedata in results)
                            {
                                Console.WriteLine($"Id:{namedata.Id}");
                                Console.WriteLine($"Name: {namedata.Name}");
                                Console.WriteLine($"PhoneNo: {namedata.PhoneNumber}");
                                Console.WriteLine($"Email: {namedata.Email}");
                                Console.WriteLine($"Address: {namedata.Address}");
                                Console.WriteLine($"Category: {namedata.Category}");
                                Console.WriteLine($"DateAdded: {namedata.date}");
                                Console.WriteLine($"Favorite: {namedata.IsFavorite}");
                                Console.WriteLine($"Total Contacts : {contactBook.TotalContacts}");
                                Console.WriteLine($"Last Modified  : {contactBook.LastModified}\n");
                            }

                        }
                        else
                        {
                            Console.WriteLine("No Contact Found");
                        }
                        break;

                    }
                } catch (Exception e)
                {
                    Console.WriteLine("Error: Search With Name" + e.Message);
                }
            }

            public void SearchWithPhoneNumber()
            {

                try
                {
                    string FilePath = "D:\\meet\\Contacts.json";

                    string jsonData = File.ReadAllText(FilePath);

                    ContactBook contactBook = JsonConvert.DeserializeObject<ContactBook>(jsonData);

                    while (true)
                    {
                        Console.WriteLine("Enter PhoneNumber to Search Contacts:");
                        string PhNumber = Console.ReadLine();

                        long PhoneNumber = Convert.ToInt64(PhNumber);

                        if (String.IsNullOrWhiteSpace(PhNumber))
                        {
                            Console.WriteLine("PhoneNumber Cannot Empty or Null");
                            continue;
                        }

                        var results = contactBook.Contacts.Where(c => c.PhoneNumber.Equals(PhNumber, StringComparison.OrdinalIgnoreCase)).ToList();

                        if (results.Count > 0)
                        {
                            Console.WriteLine("Contact found\n");

                            foreach (var namedata in results)
                            {
                                Console.WriteLine($"Id:{namedata.Id}");
                                Console.WriteLine($"Name: {namedata.Name}");
                                Console.WriteLine($"PhoneNo: {namedata.PhoneNumber}");
                                Console.WriteLine($"Email: {namedata.Email}");
                                Console.WriteLine($"Address: {namedata.Address}");
                                Console.WriteLine($"Category: {namedata.Category}");
                                Console.WriteLine($"DateAdded: {namedata.date}");
                                Console.WriteLine($"Favorite: {namedata.IsFavorite}");
                                Console.WriteLine($"Total Contacts : {contactBook.TotalContacts}");
                                Console.WriteLine($"Last Modified  : {contactBook.LastModified}\n");
                            }

                        }
                        else
                        {
                            Console.WriteLine("No Contact Found");
                        }
                        break;
                    }
                } catch (Exception e)
                {
                    Console.WriteLine("Error: for Search with PhoneNumber" + e.Message);
                }
            }


            public void SearchWithEmail()
            {
                try
                {
                    string FilePath = "D:\\meet\\Contacts.json";

                    string jsonData = File.ReadAllText(FilePath);

                    ContactBook contactBook = JsonConvert.DeserializeObject<ContactBook>(jsonData);

                    while (true)
                    {
                        Console.WriteLine("Enter Email to Search Contact :");
                        string searchEmail = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(searchEmail))
                        {
                            Console.WriteLine("Email Cannot Empty or Null");
                            continue;
                        }

                        var results = contactBook.Contacts.Where(c => c.Email.Equals(searchEmail, StringComparison.OrdinalIgnoreCase)).ToList();

                        if (results.Count > 0)
                        {
                            Console.WriteLine("Contact found\n");

                            foreach (var namedata in results)
                            {
                                Console.WriteLine($"Id:{namedata.Id}");
                                Console.WriteLine($"Name: {namedata.Name}");
                                Console.WriteLine($"PhoneNo: {namedata.PhoneNumber}");
                                Console.WriteLine($"Email: {namedata.Email}");
                                Console.WriteLine($"Address: {namedata.Address}");
                                Console.WriteLine($"Category: {namedata.Category}");
                                Console.WriteLine($"DateAdded: {namedata.date}");
                                Console.WriteLine($"Favorite: {namedata.IsFavorite}");
                                Console.WriteLine($"Total Contacts : {contactBook.TotalContacts}");
                                Console.WriteLine($"Last Modified  : {contactBook.LastModified}\n");
                            }

                        }
                        else
                        {
                            Console.WriteLine("No Contact Found");
                        }
                        break;
                    }
                } catch (Exception e)
                {
                    Console.WriteLine("Error: for Search with Email" + e.Message);
                }
            }


            public void UpdateContact()
            {
                Contact c = new Contact();
                ContactBook cb = new ContactBook();

                string filePath = @"D:\meet\Contacts.json";

                string jsonData = File.ReadAllText(filePath);

                ContactBook contactBook = JsonConvert.DeserializeObject<ContactBook>(jsonData);


                Console.Write("Enter Contact Id to Update Contact Field: ");
                string id = Console.ReadLine();

                if (!Guid.TryParse(id, out Guid contactid))
                {
                    Console.WriteLine("Invalid ID Format");
                    return;
                }

              
                Contact contact = contactBook.Contacts.FirstOrDefault(c1 => c1.Id == contactid);


                Console.WriteLine("\n Update Fields Menu");
                Console.WriteLine("1. Name");
                Console.WriteLine("2. Phone Number");
                Console.WriteLine("3. Email");
                Console.WriteLine("4. Address");
                Console.WriteLine("5. Category");

                Console.Write("Select Field to Update: ");
                int ch = Convert.ToInt32(Console.ReadLine());


                try 
                { 
                    switch (ch)
                    { 
                        case 1:

                            do
                            {
                                Console.Write("Enter New Name: ");
                                string name = Console.ReadLine();

                                if (string.IsNullOrWhiteSpace(name))
                                    Console.WriteLine("Name cannot be empty.");


                                if (!name.All(char.IsLetter) && !name.Contains(" "))
                                    Console.WriteLine("Name can contain only letters and spaces.");

                                contact.Name = name;

                            } while (string.IsNullOrWhiteSpace(contact.Name));
                            break;

                        case 2:

                            do
                            {
                                Console.WriteLine("Enter New Phone Number: ");
                                string phone = Console.ReadLine();

                                if (string.IsNullOrWhiteSpace(contact.PhoneNumber))
                                {
                                    Console.WriteLine("PhoneNumber can not Empty");
                                }

                                bool phoneExists = contactBook.Contacts.Any(x => x.PhoneNumber == c.PhoneNumber);

                                if (phoneExists)
                                {
                                    Console.WriteLine("Phone Number already exists.");
                                    continue;
                                }

                                string pattern = @"^[6-9]\d{9}$";

                                if (Regex.IsMatch(phone, pattern))
                                {
                                    Console.WriteLine("Valid");
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid PhoneNumber Format");
                                }
                                contact.PhoneNumber = phone;
                            } while (string.IsNullOrWhiteSpace(contact.PhoneNumber));
                            break;


                        case 3:
                            do
                            {
                                Console.WriteLine("Enter New Email: ");
                                string email = Console.ReadLine();

                                bool phoneExists = contactBook.Contacts.Any(x => x.PhoneNumber == c.PhoneNumber);

                                if (phoneExists)
                                {
                                    Console.WriteLine("Phone Number already exists.");
                                    continue;
                                }

                                string patternEmail = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

                                if (Regex.IsMatch(email, patternEmail))
                                {
                                    Console.WriteLine("Valid Emaill");
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid Email Formate");
                                }
                                contact.Email = email;

                            } while (string.IsNullOrWhiteSpace(contact.Email));
                            break;

                        case 4:
                            do
                            {
                                Console.WriteLine("Enter New Address: ");
                                string address = Console.ReadLine();

                                if (string.IsNullOrWhiteSpace(address))
                                {
                                    Console.WriteLine("Address Can not Null or Empty");
                                }

                                contact.Address = address;
                            } while (string.IsNullOrWhiteSpace(contact.Address));
                            break;


                        case 5:

                            do
                            {
                                Console.WriteLine("Select New Category:");
                                Console.WriteLine("1. Family");
                                Console.WriteLine("2. Friend");
                                Console.WriteLine("3. Work");
                                Console.WriteLine("4. Other");

                                Console.WriteLine("Select a category:");
                                int option = Convert.ToInt32(Console.ReadLine());


                                switch (option)
                                {
                                    case 1:
                                        contact.Category = "Family";
                                        break;
                                    case 2:
                                        contact.Category = "Friend";
                                        break;
                                    case 3:
                                        contact.Category = "Work";
                                        break;
                                    case 4:
                                        contact.Category = "Other";
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice. Please select a valid category.");
                                        continue;
                                }
                                break;

                            } while (true);
                            break;
                        case 6:
                        default:
                            Console.WriteLine("Invalid Choice");
                            break;
                    }
                } catch (Exception e)
                {
                    Console.WriteLine("Error: Updating Contact Fields" + e.Message);
                }

                string FilePath = "D:\\meet\\Contacts.json";

                contactBook.LastModified = DateTime.Now;
                contactBook.TotalContacts = contactBook.Contacts.Count;


                string updateJsondata = JsonConvert.SerializeObject(contactBook, Newtonsoft.Json.Formatting.Indented);

                File.WriteAllText(FilePath, updateJsondata);
                Console.WriteLine("Contact Updated Successfully.");
            }


            public void DeleteContact()
            {
                try
                {
                    string filePath = "D:\\meet\\Contacts.json";
                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine("File Not Found.");
                        return;
                    }


                    Console.WriteLine("Enter Contact Id to Delete");
                    string id = Console.ReadLine();

                    if (!Guid.TryParse(id, out Guid contactId))
                    {
                        Console.WriteLine("Invalid ID Format");
                        return;
                    }

                    string jsonData = File.ReadAllText(filePath);

                    ContactBook contactBook = JsonConvert.DeserializeObject<ContactBook>(jsonData);

                    Contact deleteid = contactBook.Contacts.FirstOrDefault(c2 => c2.Id == contactId);

                    if (deleteid == null)
                    {
                        Console.WriteLine("Contact not found");
                        return;
                    }

                    if (contactBook == null || contactBook.Contacts.Count == 0)
                    {
                        Console.WriteLine("No Contacts Found.");
                        return;
                    }

                    contactBook.Contacts.Remove(deleteid);

                    string FilePath = "D:\\meet\\Contacts.json";

                    contactBook.LastModified = DateTime.Now;
                    contactBook.TotalContacts = contactBook.Contacts.Count;


                    string updateJsondata = JsonConvert.SerializeObject(contactBook, Newtonsoft.Json.Formatting.Indented);

                    File.WriteAllText(FilePath, updateJsondata);
                    Console.WriteLine("Contact Deleted Successfully.");
                } catch (Exception e)
                {
                    Console.WriteLine("Error: Deleting Contact" + e.Message);
                }
                
                    
             }

            public void DisplayCetegoryContact()
            {
                try
                {
                    string filePath = @"D:\meet\Contacts.json";

                    string jsonData = File.ReadAllText(filePath);

                    ContactBook contactBook =JsonConvert.DeserializeObject<ContactBook>(jsonData);

                    Console.WriteLine("Select Category:");
                    Console.WriteLine("1. Family");
                    Console.WriteLine("2. Friend");
                    Console.WriteLine("3. Work");
                    Console.WriteLine("4. Other");

                    Console.WriteLine("Enter choice for Category Contact");
                    int choice = Convert.ToInt32(Console.ReadLine());

                    string category = "";

                    switch (choice)
                    {
                        case 1:
                            category = "Family";
                            break;
                        case 2:
                            category = "Friend";
                            break;
                        case 3:
                            category = "Work";
                            break;
                        case 4:
                            category = "Other";
                            break;
                        default:
                            Console.WriteLine("Invalid Choice");
                            return;
                    }

                    var filterContacts = contactBook.Contacts.Where(c => c.Category.Equals(category,StringComparison.OrdinalIgnoreCase)).ToList();


                    Console.WriteLine($"\n{category} Contacts:\n");

                    if (filterContacts.Count == 0)
                    {
                        Console.WriteLine($"No {category} Contacts Found.");
                        return;
                    }

                    foreach (var contact in filterContacts)
                    {
                        Console.WriteLine($"Id : {contact.Id}");
                        Console.WriteLine($"Name : {contact.Name}");
                        Console.WriteLine($"PhoneNumber : {contact.PhoneNumber}");
                        Console.WriteLine($"Email : {contact.Email}");
                        Console.WriteLine($"Address : {contact.Address}");
                        Console.WriteLine($"Category : {contact.Category}");
                        Console.WriteLine($"DateAdded : {contact.date}");
                        Console.WriteLine($"IsFavorite : {contact.IsFavorite}");
                        Console.WriteLine($"Total Contacts : {contactBook.TotalContacts}");
                        Console.WriteLine($"Last Modified  : {contactBook.LastModified}\n");
                    }
                } catch (Exception e)
                {
                    Console.WriteLine("Error: Display Category Contact"+ e.Message);
                }
                
            }


            public void BackupAndRestore()
            {
                while (true)
                {
                    Console.WriteLine("\n1. Backup");
                    Console.WriteLine("2. Restore");
                    Console.WriteLine("3. Exit");

                    Console.Write("Enter Choice: ");
                    int choice = Convert.ToInt32(Console.ReadLine());

                    switch (choice)
                    {
                        case 1:

                            try
                            {
                                string SourceFile = "D:\\meet\\Contacts.Json";
                                string BackupFile = "D:\\meet\\Backup\\Contacts_Backup.Json";

                                if (!File.Exists(SourceFile))
                                {
                                    Console.WriteLine("Sourcefile not Found");
                                }

                                Directory.CreateDirectory("D:\\meet\\Backup");

                                File.Copy(SourceFile, BackupFile, true);

                                Console.WriteLine("Backup created successfully.");
                            } catch (Exception e)
                            {
                                Console.WriteLine("Error: Backup File"+e.Message);
                            }
                            break;

                        case 2:

                            try
                            {
                                string backup = $"D:\\meet\\Backup\\Contacts_Backup.Json";
                                string dest = $"D:\\meet\\Contacts.Json";

                                if(!File.Exists(backup))
                                {
                                    Console.WriteLine("Backup file not Found");
                                }

                                File.Copy(backup, dest, true);
                                Console.WriteLine("File restored");
                            }catch (Exception e)
                            {
                                Console.WriteLine("Error: File Restore from dest"+e.Message);
                            }
                            
                            break;

                        case 3:
                            return;

                        default:
                            Console.WriteLine("Invalid Choice");
                            break;
                    }
                }
            }
                
        
            public void ViewContacts()
            {

                try
                {
                    string FilePath = "D:\\meet\\Contacts.json";

                    if (!File.Exists(FilePath))
                    {
                        Console.WriteLine("File Not Found");
                    }


                    string jsonData = File.ReadAllText(FilePath);

                    ContactBook contactBook = JsonConvert.DeserializeObject<ContactBook>(jsonData);

                    if (contactBook == null || contactBook.Contacts.Count == 0)
                    {
                        Console.WriteLine("Contacts Not Found");
                        return;
                    }

                    else
                    {
                        foreach (Contact contact in contactBook.Contacts)
                        {
                            Console.WriteLine($"Id : {contact.Id}");
                            Console.WriteLine($"Name : {contact.Name}");
                            Console.WriteLine($"PhoneNumber : {contact.PhoneNumber}");
                            Console.WriteLine($"Email : {contact.Email}");
                            Console.WriteLine($"Address : {contact.Address}");
                            Console.WriteLine($"Category : {contact.Category}");
                            Console.WriteLine($"DateAdded : {contact.date}");
                            Console.WriteLine($"IsFavorite : {contact.IsFavorite}");
                            Console.WriteLine($"Total Contacts : {contactBook.TotalContacts}");
                            Console.WriteLine($"Last Modified  : {contactBook.LastModified}\n");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: View All Contact"+ e.Message);
                }

            }

            static void Main(string[] args)
            {
                ContactBook cb = new ContactBook();

                try
                {
                    do
                    {
                        int choice;

                        Console.WriteLine("JSON Contact Book Manager");
                        Console.WriteLine("1. Add New Contact: ");
                        Console.WriteLine("2. View Contacts: ");
                        Console.WriteLine("3. Search Contact With Name: ");
                        Console.WriteLine("4. Search With PhoneNumber :");
                        Console.WriteLine("5. Search With Email :");
                        Console.WriteLine("6. Update Contact with Existing Id: ");
                        Console.WriteLine("7. Delete Contact with Existing Id: ");
                        Console.WriteLine("8. Categorize contacts (Family, Friends, Work, Other): ");
                        Console.WriteLine("9. Backup and Restore File: ");
                        Console.WriteLine("10. Exit");

                        Console.Write("Enter your choice: ");
                        choice = Convert.ToInt32(Console.ReadLine());

                        switch (choice)
                        {
                            case 1:
                                cb.AddNewContact();
                                break;
                            case 2:
                                cb.ViewContacts();
                                break;
                            case 3:
                                cb.SearchWithName();
                                break;
                            case 4:
                                cb.SearchWithPhoneNumber();
                                break;
                            case 5:
                                cb.SearchWithEmail();
                                break;
                            case 6:
                                cb.UpdateContact();
                                break;
                            case 7:
                                cb.DeleteContact();
                                break;
                            case 8:
                                cb.DisplayCetegoryContact();
                                break;
                            case 9:
                                cb.BackupAndRestore();
                                break;
                            case 10:
                                Console.WriteLine("Exit..");
                                break;
                            default:
                                Console.WriteLine("Invalid choice. Please try again.");
                                break;
                        }
                    } while (true);
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Error: String Formating " + e.Message);
                }
            }
        }
    }
}
