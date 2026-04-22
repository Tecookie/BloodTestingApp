using System.Linq;
using System.Windows;
using BloodTestingApp.Entities;
using BloodTestingApp.Pages.Admin; // Namespace của trang quản lý

namespace BloodTestingApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                txtMessage.Text = "Vui lòng nhập đầy đủ thông tin!";
                return;
            }

            using (var context = new BloodTestManagementContext())
            {
                // Kiểm tra thông tin đăng nhập
                var user = context.Users
                    .FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

                if (user != null)
                {
                    // Kiểm tra Role
                    if (user.Role?.ToUpper() == "ADMIN")
                    {
                        // Mở thẳng trang UserManagement
                        UserManagement adminWindow = new UserManagement();
                        adminWindow.Show();

                        // Đóng cửa sổ Login
                        this.Close();
                    }
                    else
                    {
                        txtMessage.Text = "Bạn không có quyền truy cập vào khu vực Admin.";
                        txtMessage.Foreground = System.Windows.Media.Brushes.Red;
                    }
                }
                else
                {
                    txtMessage.Text = "Sai tài khoản hoặc mật khẩu!";
                    txtMessage.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
        }
    }
}