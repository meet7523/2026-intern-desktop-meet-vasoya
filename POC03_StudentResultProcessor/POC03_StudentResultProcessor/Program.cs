using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC03_StudentResultProcessor
{
    internal class Program
    {
        class Student
        {
            public int RollNumber { get; set; }
            public string Name { get; set; }
            public int MathsMarks { get; set; }
            public int ScienceMarks { get; set; }
            public int EnglishMarks { get; set; }

            public int Total { get; set; }

            public double Percentage { get; set; }
            public string Grade { get; set; }


            public void AutoCalculate()
            {
                Total = MathsMarks + ScienceMarks + EnglishMarks;

                Percentage = Total / 3.0;

                if(Percentage >= 90)
                {
                    Grade = "A";
                }
                else if (Percentage >= 75 && Percentage <= 89)
                {
                    Grade = "B";
                }
                else if (Percentage >= 60 &&  Percentage <= 74)
                {
                    Grade = "C";
                }
                else if (Percentage >= 40 && Percentage <= 59)
                {
                    Grade = "D";
                }
                else if (Percentage <= 40)

                {
                    Grade= "F";
                }
                else
                {
                    Console.WriteLine("You are fail.");
                }
            }
        }

        class College
        {
            List<Student> student = new List<Student>();
          

            public void AddStudent()
            {
                Student s = new Student();

                Console.WriteLine("Enter Student Roll No :");
                s.RollNumber = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Enter Student Name :");
                s.Name = (Console.ReadLine());


                do
                {
                    Console.WriteLine("Enter Maths Marks :");
                    s.MathsMarks = Convert.ToInt32(Console.ReadLine());

                    if (s.MathsMarks <= 0 || s.MathsMarks >= 100)
                    {
                        Console.WriteLine("Enter Marks Between (0 - 100)");
                    }
                } while (s.MathsMarks <= 0 || s.MathsMarks >= 100);
                do
                {
                    Console.WriteLine("Enter Science Marks :");
                    s.ScienceMarks = Convert.ToInt32(Console.ReadLine());

                    if (s.ScienceMarks < 0 || s.ScienceMarks > 100)
                    {
                        Console.WriteLine("Enter Marks Between (0 - 100)");
                    }
                } while (s.ScienceMarks < 0 || s.ScienceMarks > 100);

                do
                {
                    Console.WriteLine("Enter English Marks :");
                    s.EnglishMarks = Convert.ToInt32(Console.ReadLine());

                    if (s.EnglishMarks < 0 || s.EnglishMarks > 100)
                    {
                        Console.WriteLine("Enter Marks Between (0 - 100)");
                    }
                } while (s.EnglishMarks < 0 || s.EnglishMarks > 100);

                    student.Add(s);

                    Console.WriteLine("Student Successfully Added");

            }

            public void SearchStudent()
            {
                Console.WriteLine("Enter Student Roll no to Search Result : ");
                int searchrollno = Convert.ToInt32(Console.ReadLine());

                var studentdata = student.FirstOrDefault(e => e.RollNumber == searchrollno);

                if (studentdata != null)
                {
                    DisplayStudent();
                }
                else
                {
                    Console.WriteLine("Student Not Found");
                }

            }

            public void DisplayStudent()
            {
               

                if (student.Count == 0)
                {
                    Console.WriteLine("Employee Not Found");
                }

                foreach (Student s in student)
                {
                    s.AutoCalculate();
                }
                

                Student maths = student.OrderByDescending(s1 => s1.MathsMarks).First();
                Student science = student.OrderByDescending(s2 => s2.ScienceMarks).First();
                Student english = student.OrderByDescending(s3 => s3.EnglishMarks).First();

                double averagePercentage = student.Average(s => s.Percentage);

                Student highestMarks = student.OrderByDescending(s => s.Total).First();

                var gradeCount = student.GroupBy(s => s.Grade).OrderBy(g => g.Key);

                Console.WriteLine("===== Student Result Sheet =====\n");
                Console.WriteLine("Roll No\t Name\t Maths\t Science\t English\t Total\t Percentage\t Grade \n");
                Console.WriteLine("-------------------------------------------------------------------------------------------");

                foreach(Student s in student)
                {
                    Console.WriteLine($"{s.RollNumber}\t{s.Name}\t {s.MathsMarks}\t {s.ScienceMarks}\t   \t{s.EnglishMarks}\t           {s.Total}\t     {s.Percentage.ToString("F2")}\t      {s.Grade}");
                }

                Console.WriteLine("_____________________________________________________________________________________________\n");

               

                Console.WriteLine("===== Class Statistics ===== \n");
                Console.WriteLine("Total Student:" + student.Count);
                Console.WriteLine($"Average Percentage: {averagePercentage.ToString("F2")}%");
                Console.WriteLine($"Highest Total:{highestMarks.Total} ({highestMarks.Name})\n");

                Console.WriteLine("Subject Toppers:\n");
                Console.WriteLine($"-Maths: {maths.Name} ({maths.MathsMarks})");
                Console.WriteLine($"-Science: {science.Name} ({science.ScienceMarks})");
                Console.WriteLine($"-English: {english.Name} ({english.EnglishMarks})\n");


                Console.WriteLine("Grade Distribution: \n");
                foreach (var grade in gradeCount)
                {
                    Console.WriteLine($"Grade {grade.Key} : {grade.Count()} student(s)");
                }

            }
        }
        static void Main()
        {
            //Student s = new Student();

            College c = new College();
            int Choice;

            do
            {
                Console.WriteLine("===== Student Result Processor Systemn =====");
                Console.WriteLine("1.Add Student");
                Console.WriteLine("2.Display Student");
                Console.WriteLine("3.Search Student Result Based on Roll No");
                Console.WriteLine("4.Exit...");

                Console.WriteLine("Enter Your Choice");
                Choice = Convert.ToInt32(Console.ReadLine());

                switch (Choice)
                {
                    case 1:
                        c.AddStudent();
                        break;
                    case 2:
                        c.DisplayStudent();
                        break;
                    case 3:
                        c.SearchStudent();
                        break;
                    case 4:
                        Console.WriteLine("Exit...");
                        break;
                    default:
                        Console.WriteLine("Invalid");
                        break;
                }
            } while (Choice != 4);
        }
    }
}
