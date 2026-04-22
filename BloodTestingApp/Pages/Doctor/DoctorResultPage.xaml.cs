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
    /// Interaction logic for DoctorResultPage.xaml
    /// </summary>
    public partial class DoctorResultPage : Page
    {
        public class AppointmentDisplay
        {
            public int AppointmentId { get; set; }
            public string DisplayText { get; set; }
        }
        public int? SelectedAppointmentId { get; set; }
        private int currentDoctorId = 1; // Simulate logged-in doctor ID
        public DoctorResultPage(int doctorId)
        {
            InitializeComponent();
            currentDoctorId = doctorId;
            LoadAppointments();
            LoadDiseases();
        }

        public void LoadAppointments()
        {
            using (var context = new BloodTestManagementContext())
            {
                var data = context.Appointments
                    .Include(r => r.Customer)
                    .Include(r => r.AssignedDoctor)
                    .Where(a => a.AssignedDoctorId == currentDoctorId
                             && a.Status == "ASSIGNED")
                    .Select(a => new AppointmentDisplay
                    {
                        AppointmentId = a.Id,
                        DisplayText = $"{a.Customer.FullName} - {a.AppointmentDate:dd/MM HH:mm}"
                    })
                    .ToList();

                cbAppointments.ItemsSource = data;
            }
        }

        public void LoadDiseases()
        {
            using (var context = new BloodTestManagementContext())
            {
                lbDiseases.ItemsSource = context.Diseases.ToList();
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (cbAppointments.SelectedValue == null)
            {
                MessageBox.Show("Chọn lịch trước!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtResult.Text))
            {
                MessageBox.Show("Nhập kết quả!");
                return;
            }

            var selected = cbAppointments.SelectedItem as AppointmentDisplay;

            if (selected == null)
            {
                MessageBox.Show("Chọn lịch trước!");
                return;
            }

            int appointmentId = selected.AppointmentId;
            using (var context = new BloodTestManagementContext())
            {
                // check đã có kết quả chưa
                var exist = context.BloodTestResults
                    .FirstOrDefault(r => r.AppointmentId == appointmentId);

                if (exist != null)
                {
                    MessageBox.Show("Lịch này đã có kết quả!");
                    return;
                }

                // tạo result
                var result = new BloodTestResult
                {
                    AppointmentId = appointmentId,
                    DoctorId = currentDoctorId,
                    ResultText = txtResult.Text
                };

                context.BloodTestResults.Add(result);
                context.SaveChanges(); // để có ResultId

                int resultId = result.Id;

                // lấy list disease đã chọn
                var selectedDiseases = lbDiseases.SelectedItems
                    .Cast<Disease>()
                    .ToList();

                foreach (var d in selectedDiseases)
                {
                    context.ResultDiseases.Add(new ResultDisease
                    {
                        ResultId = resultId,
                        DiseaseId = d.Id
                    });
                }

                // update appointment -> DONE
                var appointment = context.Appointments
                    .FirstOrDefault(a => a.Id == appointmentId);

                if (appointment != null)
                {
                    appointment.Status = "DONE";
                }

                context.SaveChanges();
            }

            MessageBox.Show("Lưu thành công!");

            // reload UI
            txtResult.Text = "";
            LoadAppointments();
        }
    }
}
