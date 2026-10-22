using POC15__TaskManagementApp.Commands;
using POC15__TaskManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace POC15__TaskManagementApp.ViewModels
{
    public class CalendarViewModel : BaseViewModel
    {
        private DateTime _displayedMonth;
        private DateTime? _selectedDate;
        private bool _isOpen;
        public DateTime MinimumSelectableDate => DateTime.Today;

        public ObservableCollection<CalendarDay> Days { get; }
            = new ObservableCollection<CalendarDay>();

        public ObservableCollection<string> WeekDays { get; }
            = new ObservableCollection<string>
            {
                "Su",
                "Mo",
                "Tu",
                "We",
                "Th",
                "Fr",
                "Sa"
            };

        public DateTime DisplayedMonth
        {
            get => _displayedMonth;

            set
            {
                if (_displayedMonth == value)
                    return;

                _displayedMonth = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(MonthYear));

                GenerateCalendar();
            }
        }

        public string MonthYear =>
            DisplayedMonth.ToString("MMMM yyyy");

        public DateTime? SelectedDate
        {
            get => _selectedDate;

            set
            {
                if (_selectedDate == value)
                    return;

                _selectedDate = value;

                foreach (var calendarDay in Days)
                {
                    calendarDay.IsSelected =
                        value.HasValue &&
                        calendarDay.Date.Date == value.Value.Date;
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(Days));

                DateSelected?.Invoke(
                    this,
                    EventArgs.Empty);
            }
        }

        public bool IsOpen
        {
            get => _isOpen;

            set
            {
                if (_isOpen == value)
                    return;

                _isOpen = value;

                OnPropertyChanged();
            }
        }

        public ICommand OpenCalendarCommand { get; }

        public ICommand CloseCalendarCommand { get; }

        public ICommand PreviousMonthCommand { get; }

        public ICommand NextMonthCommand { get; }

        public ICommand SelectDayCommand { get; }

        public event EventHandler DateSelected;

        public CalendarViewModel()
        {
            DisplayedMonth = DateTime.Today;

            OpenCalendarCommand =
                new RelayCommand(
                    _ => IsOpen = !IsOpen);

            CloseCalendarCommand =
                new RelayCommand(
                    _ => IsOpen = false);

            PreviousMonthCommand =
                new RelayCommand(
                    _ => PreviousMonth());

            NextMonthCommand =
                new RelayCommand(
                    _ => NextMonth());

            SelectDayCommand =
                new RelayCommand(
                    day => SelectDay(day as CalendarDay));

            GenerateCalendar();
        }

        private void PreviousMonth()
        {
            DisplayedMonth =
                DisplayedMonth.AddMonths(-1);
        }

        private void NextMonth()
        {
            DisplayedMonth =
                DisplayedMonth.AddMonths(1);
        }

        private void SelectDay(CalendarDay day)
        {
            if (day == null)
                return;

            if (!day.IsSelectable)
                return;

            SelectedDate = day.Date;

            IsOpen = false;
        }


        private void GenerateCalendar()
        {
            Days.Clear();

            DateTime firstDay =
                new DateTime(
                    DisplayedMonth.Year,
                    DisplayedMonth.Month,
                    1);

            int offset =
                (int)firstDay.DayOfWeek;

            DateTime startDate =
                firstDay.AddDays(-offset);

            for (int i = 0; i < 42; i++)
            {
                DateTime date =
                    startDate.AddDays(i);

                bool isCurrentMonth =
                    date.Month == DisplayedMonth.Month &&
                    date.Year == DisplayedMonth.Year;

                var day = new CalendarDay
                {
                    Date = date,

                    IsCurrentMonth = isCurrentMonth,

                    // Today = selectable
                    // Future = selectable
                    // Previous = disabled
                    IsSelectable =
                        date.Date >= DateTime.Today,

                    // Used by your XAML Today trigger
                    IsToday =
                        date.Date == DateTime.Today,

                    IsSelected =
                        SelectedDate.HasValue &&
                        SelectedDate.Value.Date == date.Date
                };

                Days.Add(day);
            }

            OnPropertyChanged(nameof(Days));
        }

    }
}
