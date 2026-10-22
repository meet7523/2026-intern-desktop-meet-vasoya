using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace POC02_EmployeeManagement
{
    internal class Program
    {
        class Employee
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Department { get; set; }
            public decimal Salary { get; set; }
            public DateTime JoiningDate { get; set; }
        }


        class Elms
        {

            static List<Employee> list = new List<Employee>();

            static int count = 1001;


            static void AddEmployee()
            {
                try
                {
                    Employee emp = new Employee();

                    count++;
                    emp.Id = count;

                    do
                    {
                        Console.WriteLine("Enter Employee Name :");
                        emp.Name = Console.ReadLine().Trim();

                        if (string.IsNullOrWhiteSpace(emp.Name))
                        {
                            Console.WriteLine("Employee Name can not be Empty");

                        }
                    } while (string.IsNullOrWhiteSpace(emp.Name));

                    do
                    {
                        Console.WriteLine("Enter Your Department :");
                        emp.Department = Console.ReadLine().Trim();

                        if (string.IsNullOrEmpty(emp.Department))
                        {
                            Console.WriteLine("Employee Department can not be Empty ");

                        }
                    } while (string.IsNullOrWhiteSpace(emp.Department));

                    do
                    {
                        Console.WriteLine("Enter Your Salary : ");
                        emp.Salary = Convert.ToDecimal(Console.ReadLine());

                        if (emp.Salary <= 0)
                        {
                            Console.WriteLine("Enter Positive Salary not Nagative");
                        }
                    } while (emp.Salary <= 0);

                    Console.WriteLine("Enter Your JoiningDate");
                    emp.JoiningDate = Convert.ToDateTime(Console.ReadLine());

                    list.Add(emp);

                    Console.WriteLine("Employee Successfully Added.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: Employee Input Formate not Valid" + e.Message);
                }

            }

            static void DisplayEmployee()
            {

                try
                {
                    if (list.Count == 0)
                    {
                        Console.WriteLine("Employee Not Found");
                    }
                    else
                    {
                        Console.WriteLine("======== All Employee ========\n");
                        Console.WriteLine("ID \t Name \tDepartment \t\t\t Salary \t\t Joining Date\n");
                        Console.WriteLine("_________________________________________________________________________________________");


                        foreach (Employee emp in list)
                        {

                            Console.WriteLine($"{emp.Id}\t {emp.Name}\t {emp.Department}\t\t\t {emp.Salary}\t\t {emp.JoiningDate}");

                        }

                        Console.WriteLine("__________________________________________________________________________________________");
                        Console.WriteLine("Total Employee:" + list.Count);

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: Employee not Display" + e.Message);
                }
            }

            static void Searchemployee()
            {
                try
                {
                    Console.WriteLine("Enter ID to Search : ");
                    int searchId = Convert.ToInt32(Console.ReadLine());

                    var empData = list.FirstOrDefault(e => e.Id == searchId);

                    if (empData != null)
                    {
                        Console.WriteLine(empData.Name);
                    }
                    else
                    {
                        Console.WriteLine("Employee Not Found");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: Search Employee" + e.Message);
                }
            }


            static void UpdateEmployee()
            {

                try
                {
                    if (list.Count == 0)
                    {
                        Console.WriteLine("Employee List is Empty");
                        return;
                    }

                    Console.Write("Enter Employee ID to Modify: ");
                    int id = Convert.ToInt32(Console.ReadLine());

                    Employee emp = list.FirstOrDefault(e => e.Id == id);

                    if (emp == null)
                    {
                        Console.WriteLine("Employee ID Not Found");
                        return;
                    }

                    Console.WriteLine("\nUpdate Employee Field");
                    Console.WriteLine("1. Name");
                    Console.WriteLine("2. Department");
                    Console.WriteLine("3. Salary");
                    Console.WriteLine("4. Joining Date");
                    Console.Write("Enter Choice: ");

                    int choice = Convert.ToInt32(Console.ReadLine());

                    switch (choice)
                    {
                        case 1:
                            do
                            {
                                Console.Write("Enter New Name: ");
                                emp.Name = Console.ReadLine();

                                if (string.IsNullOrWhiteSpace(emp.Name))
                                    Console.WriteLine("Name cannot be empty.");
                            }
                            while (string.IsNullOrWhiteSpace(emp.Name));
                            break;

                        case 2:
                            do
                            {
                                Console.Write("Enter New Department: ");
                                emp.Department = Console.ReadLine();

                                if (string.IsNullOrWhiteSpace(emp.Department))
                                    Console.WriteLine("Department cannot be empty.");
                            }
                            while (string.IsNullOrWhiteSpace(emp.Department));
                            break;

                        case 3:
                            decimal salary;
                            do
                            {
                                Console.Write("Enter New Salary: ");
                            }
                            while (!decimal.TryParse(Console.ReadLine(), out salary) || salary <= 0);

                            emp.Salary = salary;
                            break;

                        case 4:
                            DateTime joiningDate;
                            do
                            {
                                Console.Write("Enter New Joining Date");
                            }
                            while (!DateTime.TryParse(Console.ReadLine(), out joiningDate));

                            emp.JoiningDate = joiningDate;
                            break;

                        default:
                            Console.WriteLine("Invalid Choice");
                            return;
                    }

                    Console.WriteLine("Employee Updated Successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Updating Employee" + ex.Message);
                }
            }


            static void DeleteEmployee()
            {
                try
                {
                    Console.WriteLine("\nEnter Employee ID to Delete: ");
                    int eid = Convert.ToInt32(Console.ReadLine());

                    var employeeId = list.FirstOrDefault(e => e.Id == eid);

                    if (employeeId != null)
                    {
                        list.Remove(employeeId);

                        Console.WriteLine("Employee Deleted Successfully");
                    }
                    else
                    {
                        Console.WriteLine("Employee not found");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Deleting Employee" + ex.Message);
                }
            }


            static void Main()
            {
                Employee emp = new Employee();
                int choice;

                try
                {
                    do
                    {

                        Console.WriteLine("\n=== Employee Leave Management System ===");
                        Console.WriteLine("1.Add new Employee :");
                        Console.WriteLine("2.View All Employee :");
                        Console.WriteLine("3.Search Employee By Id :");
                        Console.WriteLine("4.Update Employee Details :");
                        Console.WriteLine("5.Delete Employee :");
                        Console.WriteLine("6.Exit");

                        Console.WriteLine("Enter your Choice");
                        choice = Convert.ToInt32(Console.ReadLine());

                        switch (choice)
                        {
                            case 1:
                                AddEmployee();
                                break;

                            case 2:
                                DisplayEmployee();
                                break;

                            case 3:
                                Searchemployee();
                                break;

                            case 4:
                                UpdateEmployee();
                                break;

                            case 5:
                                DeleteEmployee();
                                break;

                            case 6:
                                Console.WriteLine("Exit");
                                break;

                            default:
                                Console.WriteLine("Invalid");
                                break;
                        }
                    } while (choice != 6);
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Error: String Formatting" + e.Message);
                }

            }
        }
    }
}

                                 

   



