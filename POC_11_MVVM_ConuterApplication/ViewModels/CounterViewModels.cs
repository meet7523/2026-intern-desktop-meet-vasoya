using POC_11_MVVM_ConuterApplication.Commands;
using POC_11_MVVM_ConuterApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;

namespace POC_11_MVVM_ConuterApplication.ViewModels
{
    internal class CounterViewModels : INotifyPropertyChanged
    {
        private int count;

        public int Count
        {
            get => count;
            set
            {
                count = value;
                OnPropertyChanged(nameof(count));       
            }
        }

        public ICommand IncrementCommand { get; }
        public ICommand DecrementCommand { get; }
        public ICommand ResetCommand { get; }

        public CounterViewModels()
        {
            Count = 0;

            IncrementCommand = new RelayCommand(Increment);
            DecrementCommand = new RelayCommand(Decrement);
            ResetCommand = new RelayCommand(Reset);
        }


        private void Increment()
        {
            Count++;
        }

        private void Decrement()
        {
          
            Count--;
        }

        private void Reset()
        {
            Count = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }


    }
}
