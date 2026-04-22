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
            public string Status { get; set; }
            public string AssignedDoctorName { get; set; }
        }
        private int currentDoctorId = 1; // Simulate logged-in doctor ID

        public DoctorPendingPage(int doctorId)
        {
            InitializeComponent();
            currentDoctorId = doctorId;
            LoadPending();
        }

        public void LoadPending()
        {
            using (var context = new BloodTestManagementContext())
            {
                var data = context.Appointments
                    .Include(a => a.Customer)
                    .Include(a => a.AssignedDoctor)
                    .Include(a => a.AppointmentRequests)
                    //.Where(a =>
                    //    // doctor này có request
                    //    a.AppointmentRequests.Any(r => r.DoctorId == currentDoctorId)
                    //    || a.AssignedDoctorId == currentDoctorId
                    //)
                    .Select(a => new PendingAppointment
                    {
                        AppointmentId = a.Id,
                        CustomerName = a.Customer.FullName,
                        AppointmentDate = a.AppointmentDate,
                        Status = a.Status,
                        AssignedDoctorName = a.AssignedDoctor != null
                            ? a.AssignedDoctor.FullName
                            : "Chưa có"
                    })
                    .OrderByDescending(a => a.AppointmentDate)
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
        private void Reject_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext as PendingAppointment;

            if (item == null) return;

            button.IsEnabled = item.Status == "PENDING";
        }
        private void Accept_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext as PendingAppointment;

            if (item == null) return;

            // chỉ cho accept khi PENDING
            button.IsEnabled = item.Status == "PENDING";
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
