using BloodTestingApp.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BloodTestingApp.Pages.Doctor
{
    /// <summary>
    /// Interaction logic for DoctorAssignedPage.xaml
    /// </summary>
    public partial class DoctorAssignedPage : Page
    {
        private int currentDoctorId = 1; // Simulate logged-in doctor ID
        public class AssignedVM
        {
            public int AppointmentId { get; set; }
            public string CustomerName { get; set; }
            public DateTime AppointmentDate { get; set; }
        }
        public DoctorAssignedPage(int doctorId)
        {
            InitializeComponent();
            currentDoctorId = doctorId;
            LoadAssigned();
        }

        public void LoadAssigned()
        {
            using (var context = new BloodTestManagementContext())
            {
                var data = context.Appointments
                    .Include(a => a.Customer)
                    .Where(a => a.AssignedDoctorId == currentDoctorId
                             && a.Status == "ASSIGNED")
                    .Select(a => new AssignedVM
                    {
                        AppointmentId = a.Id,
                        CustomerName = a.Customer.FullName,
                        AppointmentDate = a.AppointmentDate
                    })
                    .ToList();

                dgAssigned.ItemsSource = data;
            }
        }

        private void Result_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext as AssignedVM;

            if (item == null) return;

            // chuyển sang ResultPage
            var resultPage = new DoctorResultPage(currentDoctorId);

            resultPage.SelectedAppointmentId = item.AppointmentId;

            NavigationService.Navigate(resultPage);
        }
    }
}
