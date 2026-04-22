using BloodTestingApp.Entities;
using BloodTestingApp.Pages.Admin;
using BloodTestingApp.Pages.Doctor;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
// Thêm namespace chứa DoctorMainWindow nếu nó nằm ở thư mục khác
// using BloodTestingApp.Pages.Doctor; 

namespace BloodTestingApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void btnClose_Click(object sender, RoutedEventArgs e) => Close();
        private void btnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            // 1. Kiểm tra đầu vào rỗng
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                txtMessage.Text = "Vui lòng nhập đầy đủ tài khoản và mật khẩu!";
                return;
            }

            try
            {
                using (var context = new BloodTestManagementContext())
                {
                    // 2. Tìm User khớp Username và Password
                    var user = context.Users
                        .FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

                    if (user != null)
                    {
                        string role = user.Role?.ToUpper();

                        // 3. Phân quyền chuyển hướng
                        if (role == "ADMIN")
                        {
                            UserManagement adminWindow = new UserManagement();
                            adminWindow.Show();
                            this.Close();
                        }
                        else if (role == "DOCTOR")
                        {
                            // Mở trang dành cho Bác sĩ
                            var doctor = context.Doctors
                            .FirstOrDefault(d => d.UserId == user.Id);

                            DoctorMainWindow doctorWindow = new DoctorMainWindow(doctor.Id);
                            doctorWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            txtMessage.Text = "Tài khoản của bạn không có quyền truy cập khu vực này!";
                            txtMessage.Foreground = System.Windows.Media.Brushes.Red;
                        }
                    }
                    else
                    {
                        txtMessage.Text = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                        txtMessage.Foreground = System.Windows.Media.Brushes.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi kết nối DB", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}