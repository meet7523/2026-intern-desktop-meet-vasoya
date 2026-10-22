using System;

namespace TaskManagement.ViewModels
{
    public class Calender
    {
        public DateTime Date { get; }

        public int Day => Date.Day;

        public bool IsToday =>
            Date.Date == DateTime.Today;

        public bool IsPast =>
            Date.Date < DateTime.Today;

        public bool IsSelectable =>
            Date.Date >= DateTime.Today;

        public Calendar(DateTime date)
        {
            Date = date;
        }
    }
}
