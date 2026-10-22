using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC01_ConsoleCalculator
{
    internal class Program
    {
        static void Main()
        {
            int choice;
            double num1, num2, result;

            double Addition(double a, double b)
            {
                return a + b;
            }
            double Subtraction(double a, double b)
            {
                return a - b;
            }
            double Multiplication(double a, double b)
            {
                return a * b;
            }
            double Division(double a, double b)
            {
                if (b == 0)
                {
                    Console.WriteLine("Division by zero is not allowed.");
                    return 0;
                }
                return a / b;
            }

            do
            {
                Console.WriteLine("Simple Calculator");
                Console.WriteLine("1. Addition");
                Console.WriteLine("2. Subtraction");
                Console.WriteLine("3. Multiplication");
                Console.WriteLine("4. Division");
                Console.WriteLine("5. Exit");
                Console.WriteLine(" Chose Operation ( 1 - 5) :");
                choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Enter first number:");
                        num1 = Convert.ToDouble(Console.ReadLine());
                        Console.WriteLine("Enter second number:");
                        num2 = Convert.ToDouble(Console.ReadLine());
                        result = Addition(num1, num2);
                        Console.WriteLine("Addition: " + result);
                        break;

                    case 2:
                        Console.WriteLine("Enter first number:");
                        num1 = Convert.ToDouble(Console.ReadLine());
                        Console.WriteLine("Enter second number:");
                        num2 = Convert.ToDouble(Console.ReadLine());
                        result = Subtraction(num1, num2);
                        Console.WriteLine("Subtrection: " + result);
                        break;

                    case 3:
                        Console.WriteLine("Enter first number:");
                        num1 = Convert.ToDouble(Console.ReadLine());
                        Console.WriteLine("Enter second number:");
                        num2 = Convert.ToDouble(Console.ReadLine());
                        result = Multiplication(num1, num2);
                        Console.WriteLine("Maltiplication:" + result);
                        break;

                    case 4:
                        Console.WriteLine("Enter first number");
                        num1 = Convert.ToDouble(Console.ReadLine());
                        Console.WriteLine("Enter second number");
                        num2 = Convert.ToDouble(Console.ReadLine());
                        result = Division(num1, num2);
                        Console.WriteLine("Division:" + result);
                        break;

                    case 5:
                        Console.WriteLine("Exiting...");
                        break;

                    default:
                        Console.WriteLine("Invalid");
                        break;
                }

            } while (choice != 5);
        }
    }
}

