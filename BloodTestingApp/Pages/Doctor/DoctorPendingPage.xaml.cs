using BloodTestingApp.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class DoctorPendingPage : Page
    {
        public class PendingAppointment
        {
            public int AppointmentId { get; set; }
            public string CustomerName { get; set; }
            public DateTime AppointmentDate { get; set; }
        }
        private int currentDoctorId = 1; // Simulate logged-in doctor ID

        public DoctorPendingPage()
        {
            InitializeComponent();
            LoadPending();
        }

        public void LoadPending()
        {
            using (var context = new BloodTestManagementContext())
            {
                var data = context.AppointmentRequests
                    .Include(r => r.Appointment)
                    .ThenInclude(a => a.Customer)
                    .Where(r => r.DoctorId == currentDoctorId && r.Status == "PENDING")
                    .Select(r => new PendingAppointment
                    {
                        AppointmentId = r.Appointment.Id,
                        CustomerName = r.Appointment.Customer.FullName,
                        AppointmentDate = r.Appointment.AppointmentDate
                    })
                    .ToList();

                dgPending.ItemsSource = data;
            }
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext as PendingAppointment;

            if (item == null) return;

            using (var context = new BloodTestManagementContext())
            {
                // lấy appointment
                var appointment = context.Appointments
                    .FirstOrDefault(a => a.Id == item.AppointmentId);

                if (appointment == null || appointment.Status != "PENDING")
                {
                    MessageBox.Show("Lịch đã được người khác nhận!");
                    return;
                }

                // gán bác sĩ
                appointment.Status = "ASSIGNED";
                appointment.AssignedDoctorId = currentDoctorId;

                // update request của bác sĩ này
                var myRequest = context.AppointmentRequests
                    .FirstOrDefault(r => r.AppointmentId == item.AppointmentId
                                      && r.DoctorId == currentDoctorId);

                if (myRequest != null)
                {
                    myRequest.Status = "ACCEPTED";
                    myRequest.RespondedAt = DateTime.Now;
                }

                // reject tất cả bác sĩ khác
                var otherRequests = context.AppointmentRequests
                    .Where(r => r.AppointmentId == item.AppointmentId
                             && r.DoctorId != currentDoctorId)
                    .ToList();

                foreach (var r in otherRequests)
                {
                    r.Status = "REJECTED";
                    r.RespondedAt = DateTime.Now;
                }

                context.SaveChanges();
            }

            MessageBox.Show("Đã nhận lịch!");
            LoadPending();
        }
        private void Reject_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext as PendingAppointment;

            if (item == null) return;

            using (var context = new BloodTestManagementContext())
            {
                var request = context.AppointmentRequests
                    .FirstOrDefault(r => r.AppointmentId == item.AppointmentId
                                      && r.DoctorId == currentDoctorId);

                if (request != null)
                {
                    request.Status = "REJECTED";
                    request.RespondedAt = DateTime.Now;

                    context.SaveChanges();
                }
            }

            MessageBox.Show("Đã từ chối!");
            LoadPending();
        }
    }
}