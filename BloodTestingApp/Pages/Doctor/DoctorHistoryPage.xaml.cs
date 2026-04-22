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
    /// Interaction logic for DoctorHistoryPage.xaml
    /// </summary>
    public partial class DoctorHistoryPage : Page
    {
        private int currentDoctorId = 1;

        public class HistoryVM
        {
            public int AppointmentId { get; set; }
            public string CustomerName { get; set; }
            public DateTime AppointmentDate { get; set; }
            public string ResultText { get; set; }
            public string Diseases { get; set; } // nối chuỗi
        }
        public DoctorHistoryPage()
        {
            InitializeComponent();
            LoadHistory();
        }
        public void LoadHistory()
        {
            using (var context = new BloodTestManagementContext())
            {
                var data = context.BloodTestResults
                    .Include(r => r.Appointment)
                        .ThenInclude(a => a.Customer)
                    .Include(r => r.ResultDiseases)
                        .ThenInclude(rd => rd.Disease)
                    .Where(r => r.DoctorId == currentDoctorId)
                    .ToList()
                    .Select(r => new HistoryVM
                    {
                        AppointmentId = r.AppointmentId.Value,
                        CustomerName = r.Appointment.Customer.FullName,
                        AppointmentDate = r.Appointment.AppointmentDate,
                        ResultText = r.ResultText,

                        // nối danh sách bệnh thành string
                        Diseases = string.Join(", ",
                            r.ResultDiseases.Select(rd => rd.Disease.Name))
                    })
                    .ToList();

                dgHistory.ItemsSource = data;
            }
        }
    }
}
