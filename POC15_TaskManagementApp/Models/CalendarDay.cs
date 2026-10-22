using POC15__TaskManagementApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC15__TaskManagementApp.Models
{
    public class CalendarDay : BaseViewModel
    {
        public DateTime Date { get; set; }

        public string DayNumber =>
            Date.Day.ToString();

        public bool IsToday { get; set; }

        public bool IsCurrentMonth { get; set; }

        public bool IsSelectable { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;

            set
            {
                if (_isSelected == value)
                    return;

                _isSelected = value;

                OnPropertyChanged();
            }
        }
    }
}
