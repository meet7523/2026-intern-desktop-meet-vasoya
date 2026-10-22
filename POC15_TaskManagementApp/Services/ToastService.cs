using POC15__TaskManagementApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC13__TaskManagementApp.Services
{
    public class ToastService
    {
        public void Show(string message)
        {
            ToastNotification toast = new ToastNotification(message);

            toast.Show();
        }
    }
}
