using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static POC06_ConfigBasedApplication.Program;
using Microsoft.Extensions.Configuration.Json;

namespace POC06_ConfigBasedApplication
{
    internal class Program
    {
        static IConfigurationRoot configuration;
        public class AppSettings
        {
            public string ApplicationName { get; set; }
            public string Version { get; set; }
            public string Environment { get; set; }
        }

        public class DatabaseConfig
        {
            public string Provider { get; set; }
            public string ConnectionString { get; set; }
            public int MaxConnections { get; set; }
        }

        public class FeatureFlags
        {
            public bool EnableLogging { get; set; }
            public bool EnableNotifications { get; set; }
            public bool EnableAutoSave { get; set; }
        }

        public class UserPreferences
        {
            public string Theme { get; set; }
            public string Language { get; set; }
            public string DateFormat { get; set; }
        }


        public class Configuration
        {
            public AppSettings AppSettings { get; set; }
            public DatabaseConfig DatabaseConfig { get; set; }
            public FeatureFlags FeatureFlags { get; set; }
            public UserPreferences UserPreferences { get; set; }

        }


        public class AppSetting
        {
           public void UpdateConfigurationSetting()
            {
                try
                {
                    string filePath = "D:\\meet\\Config.json";

                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine("Configuration File Not Found.");
                        return;
                    }

                    string jsonData = File.ReadAllText(filePath);

                    Configuration config = JsonConvert.DeserializeObject<Configuration>(jsonData);

                    if (config == null)
                    {
                        Console.WriteLine("Configuration Setting Not Found");
                    }

                    Console.WriteLine("\nUpdate Menu");
                    Console.WriteLine("1. Application Name");
                    Console.WriteLine("2. Version");
                    Console.WriteLine("3. Environment");
                    Console.WriteLine("4. Provider");
                    Console.WriteLine("5. Connection String");
                    Console.WriteLine("6. Max Connections");
                    Console.WriteLine("7. Enable Logging");
                    Console.WriteLine("8. Enable Notifications");
                    Console.WriteLine("9. EnableAutoSave");
                    Console.WriteLine("10. Theme");
                    Console.WriteLine("11. Language");
                    Console.WriteLine("12. DateFormat");


                    Console.Write("\nSelect Field To Update: ");
                    int choice = Convert.ToInt32(Console.ReadLine());

                    switch (choice)
                    {
                        case 1:
                            Console.WriteLine("Enter New Application Name :");
                            config.AppSettings.ApplicationName = Console.ReadLine();
                            break;

                        case 2:
                            Console.WriteLine("Enter New Version :");
                            config.AppSettings.Version = (Console.ReadLine());
                            break;

                        case 3:
                            Console.WriteLine("Enter New Environment :");
                            config.AppSettings.Environment = (Console.ReadLine());
                            break;

                        case 4:
                            Console.WriteLine("Enter New Provider :");
                            config.DatabaseConfig.Provider = (Console.ReadLine());
                            break;

                        case 5:
                            Console.WriteLine("Enter New ConnectionString :");
                            config.DatabaseConfig.ConnectionString = (Console.ReadLine());
                            break;

                        case 6:
                            Console.WriteLine("Enter New MaxConnection :");
                            config.DatabaseConfig.MaxConnections = Convert.ToInt32(Console.ReadLine());
                            break;

                        case 7:
                            Console.WriteLine("Enter New EnableLogging (true/false):");
                            config.FeatureFlags.EnableLogging = Convert.ToBoolean(Console.ReadLine());
                            break;

                        case 8:
                            Console.WriteLine("Enter New Enable Notifications (true/false):");
                            config.FeatureFlags.EnableNotifications = Convert.ToBoolean(Console.ReadLine());
                            break;

                        case 9:
                            Console.WriteLine("Enter New EnableAutoSave (true/false) :");
                            config.FeatureFlags.EnableAutoSave = Convert.ToBoolean(Console.ReadLine());
                            break;

                        case 10:
                            Console.WriteLine("Enter New Theme :");
                            config.UserPreferences.Theme = Console.ReadLine();
                            break;

                        case 11:
                            Console.WriteLine("Enter New Language :");
                            config.UserPreferences.Language = Console.ReadLine();
                            break;

                        case 12:
                            Console.WriteLine("Enter New DateFormate :");
                            config.UserPreferences.DateFormat = Console.ReadLine();
                            break;

                        default:
                            Console.WriteLine("Invalid Choice");
                            return;
                    }

                    string updatedJson = JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);

                    File.WriteAllText(filePath, updatedJson);

                    Console.WriteLine("Configuration Updated Successfully.");

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: Updating Configuration record" + e.Message);
                }
            }

            

            public void ApplicationFeatureFlag()
            {
                try
                {
                    
                    string filePath = "D:\\meet\\Config.Json";

                    string jsonData = File.ReadAllText(filePath);

                    Configuration config = JsonConvert.DeserializeObject<Configuration>(jsonData);

                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine("File Not Found");
                        return;
                    }

                    
                        if (config.FeatureFlags.EnableLogging)
                        {
                            Console.WriteLine($"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] LOG : Application Started");

                        }

                        if (config.FeatureFlags.EnableNotifications)
                        {
                            Console.WriteLine("Notification : You have 3 pending Tasks.");
                        }

                        if(config.FeatureFlags.EnableAutoSave == false)
                        {
                            Console.WriteLine("Auto save is Desbled");
                        }

                        
                            if (config.UserPreferences.Theme.Equals(
                                "Dark",StringComparison.OrdinalIgnoreCase))
                            {
                                Console.BackgroundColor = ConsoleColor.Black;
                                Console.ForegroundColor = ConsoleColor.White;

                                Console.WriteLine("Dark Theme Applied");
                            }
                            else if (config.UserPreferences.Theme.Equals(
                                "Light",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                Console.BackgroundColor = ConsoleColor.White;
                                Console.ForegroundColor = ConsoleColor.Black;

                                Console.WriteLine("Light Theme Applied");
                            }
                        
                } catch (Exception e)
                {
                    Console.WriteLine("Error: Application Feature Flag"+e.Message);
                }
            }


            public void DefaultConfiguration()
            {
                string filePath = "D:\\meet\\Config.json";

                if (!File.Exists(filePath))
                {
                    Configuration defaultConfig = new Configuration
                    {
                        AppSettings = new AppSettings
                        {
                            ApplicationName = "Task Manager Pro",
                            Version = "1.0.0",
                            Environment = "Development"
                        },

                        DatabaseConfig = new DatabaseConfig
                        {
                            Provider = "SQLite",
                            ConnectionString = "Data Source=tasks.db",
                            MaxConnections = 10
                        },

                        FeatureFlags = new FeatureFlags
                        {
                            EnableLogging = true,
                            EnableNotifications = true,
                            EnableAutoSave = false
                        },

                        UserPreferences = new UserPreferences
                        {
                            Theme = "Dark",
                            Language = "English",
                            DateFormat = "yyyy-MM-dd"
                        }
                    };

                   
                    string jsonData = JsonConvert.SerializeObject(defaultConfig,Newtonsoft.Json.Formatting.Indented);

                    File.WriteAllText(filePath, jsonData);

                    Console.WriteLine("Default Configuration Created");
                }
            }


            public static void ReloadConfiguration()
            {
                configuration = new ConfigurationBuilder()
                .AddJsonFile(
                    "D:\\meet\\Config.json",
                    optional: false,
                    reloadOnChange: true)
                .Build();

                DisplayAllSettings();

                ChangeToken.OnChange(
                    () => configuration.GetReloadToken(),
                    () =>
                    {
                        Console.WriteLine("\nConfiguration Reloaded...\n");

                        DisplayAllSettings();
                    });
                Console.ReadLine();
            }

            static void DisplayAllSettings()
            {
                foreach (var section in configuration.GetChildren())
                {
                    Console.WriteLine($"\n===== {section.Key} =====");

                    foreach (var item in section.GetChildren())
                    {
                        Console.WriteLine($"{item.Key} : {item.Value}");
                    }
                }
            }



            public void ViewConfigurationFile()
            {
                
                try
                {
                    string filePath = "D:\\meet\\Config.Json";

                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine("File Not Found");
                        return;
                    }

                    string jsonData = File.ReadAllText(filePath);

                    Configuration config = JsonConvert.DeserializeObject<Configuration>(jsonData);

                    if (config == null)
                    {
                        Console.WriteLine("Configuration Setting Not Found");
                    }
                    else
                    {
                        
                            Console.WriteLine("App Settings");
                            Console.WriteLine($"Application Name:{config.AppSettings.ApplicationName}");
                            Console.WriteLine($"Version:{config.AppSettings.Version}");
                            Console.WriteLine($"Environment:{config.AppSettings.Environment}");

                            Console.WriteLine("\nDatabase Config");
                            Console.WriteLine($"Provider: {config.DatabaseConfig.Provider}");
                            Console.WriteLine($"ConnectionString :{config.DatabaseConfig.ConnectionString}");
                            Console.WriteLine($"MaxConnections:{config.DatabaseConfig.MaxConnections}");

                            Console.WriteLine("\nFeature Flags");
                            Console.WriteLine($"Enable Logging : {config.FeatureFlags.EnableLogging}");
                            Console.WriteLine($"Enable Notifications :{config.FeatureFlags.EnableNotifications}");
                            Console.WriteLine($"EnableAutoSave : {config.FeatureFlags.EnableAutoSave}");

                            Console.WriteLine("\nUserPreferences");
                            Console.WriteLine($"Theme : {config.UserPreferences.Theme}");
                            Console.WriteLine($"Language : {config.UserPreferences.Language}");
                            Console.WriteLine($"DateFormat : {config.UserPreferences.DateFormat}\n");
  
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: View All Configuration Setting" + e.Message);
                }
            }

            static void Main(string[] args)
            {
                AppSetting app = new AppSetting();

                try
                {
                    do
                    {
                        int choice;

                        Console.WriteLine("==== Configuration Manager ====");
                        Console.WriteLine("Current App: Task Manager Pro v1.0.0 \n");
                        Console.WriteLine("1. View Configuration Settings: ");
                        Console.WriteLine("2. Update Configuration Settings: ");
                        Console.WriteLine("3. Test Config-Driven Behavior and Feature Flag: ");
                        Console.WriteLine("4. Create Default Configuration: ");
                        Console.WriteLine("5. Reload Configuration");
                        Console.WriteLine("6. Exit");

                        Console.Write("Enter your choice: ");
                        choice = Convert.ToInt32(Console.ReadLine());

                        switch (choice)
                        {
                            
                            case 1:
                                app.ViewConfigurationFile();
                                break;
                            case 2:
                                app.UpdateConfigurationSetting();
                                break;
                            case 3:
                                app.ApplicationFeatureFlag();
                                break;
                            case 4:
                                app.DefaultConfiguration();
                                break;
                            case 5:
                                ReloadConfiguration();
                                break;
                            case 6:
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
    

