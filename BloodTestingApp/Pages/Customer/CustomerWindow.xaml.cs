using BloodTestingApp.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BloodTestingApp.Pages.Customer
{
    public partial class CustomerWindow : Window
    {
        private readonly BloodTestManagementContext _context = new BloodTestManagementContext();
        private int _currentUserId;

        public CustomerWindow(int userId)
        {
            InitializeComponent();
            _currentUserId = userId;
            LoadAllData();
            LoadTime();
        }
        private void LoadTime()
        {
            var times = new List<string>();

            for (int h = 7; h <= 17; h++)
            {
                times.Add($"{h}:00");
            }

            cbTime.ItemsSource = times;
        }
        private void dpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpDate.SelectedDate == null) return;

            var date = dpDate.SelectedDate.Value;

            // ❌ Chủ nhật
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                MessageBox.Show("Không làm việc Chủ Nhật!");
                dpDate.SelectedDate = null;
                return;
            }

            // ❌ quá khứ
            if (date.Date < DateTime.Today)
            {
                MessageBox.Show("Không chọn ngày trong quá khứ!");
                dpDate.SelectedDate = null;
                return;
            }

            // reload giờ theo ngày mới
            LoadTime();
        }
        private void cbTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpDate.SelectedDate == null || cbTime.SelectedItem == null)
                return;

            var selectedDate = dpDate.SelectedDate.Value;
            var hour = int.Parse(cbTime.SelectedItem.ToString().Split(':')[0]);

            // ❌ nếu hôm nay
            if (selectedDate.Date == DateTime.Today && hour <= DateTime.Now.Hour)
            {
                MessageBox.Show("Không chọn giờ trong quá khứ!");
                cbTime.SelectedItem = null;
            }
        }
        private DateTime? GetSelectedDateTime()
        {
            if (dpDate.SelectedDate == null || cbTime.SelectedItem == null)
                return null;

            var date = dpDate.SelectedDate.Value;
            var hour = int.Parse(cbTime.SelectedItem.ToString().Split(':')[0]);

            return new DateTime(date.Year, date.Month, date.Day, hour, 0, 0);
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new MainWindow();
            login.Show();

            this.Close();
        }
        private void LoadAllData()
        {
            try
            {
                var user = _context.Users
                    .Include(u => u.Customer)
                    .FirstOrDefault(u => u.Id == _currentUserId);

                if (user != null && user.Customer != null)
                {
                    // Hiển thị Profile
                    txtFullName.Text = user.Customer.FullName;
                    txtPhone.Text = user.Customer.Phone;
                    txtAddress.Text = user.Customer.Address;

                    int cusId = user.Customer.Id;

                    // Load Lịch hẹn
                    dgAppointments.ItemsSource = _context.Appointments
                        .Where(a => a.CustomerId == cusId)
                        .OrderByDescending(a => a.CreatedAt)
                        .ToList();

                    // Load Kết quả kèm danh sách bệnh (Deep Include)
                    dgResults.ItemsSource = _context.BloodTestResults
                        .Include(r => r.Doctor)
                        .Include(r => r.ResultDiseases)
                            .ThenInclude(rd => rd.Disease)
                        .Where(r => r.Appointment.CustomerId == cusId)
                        .OrderByDescending(r => r.CreatedAt)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dpDate.SelectedDate == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày xét nghiệm!", "Nhắc nhở", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var customer = _context.Customers.FirstOrDefault(c => c.UserId == _currentUserId);
                if (customer != null)
                {
                    var newApp = new Appointment
                    {
                        CustomerId = customer.Id,
                        AppointmentDate = GetSelectedDateTime().Value,
                        Status = "PENDING",
                        Note = txtNote.Text,
                        CreatedAt = DateTime.Now
                    };

                    _context.Appointments.Add(newApp);
                    _context.SaveChanges();

                    MessageBox.Show("Gửi yêu cầu đặt lịch thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Reset form và tải lại bảng lịch sử
                    txtNote.Text = "";
                    dpDate.SelectedDate = null;
                    LoadAllData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đặt lịch: " + ex.Message);
            }
        }
    }
}