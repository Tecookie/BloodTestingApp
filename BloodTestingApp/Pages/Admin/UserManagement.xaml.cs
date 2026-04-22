using BloodTestingApp.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BloodTestingApp.Pages.Admin
{
    public partial class UserManagement : Window
    {
        private readonly BloodTestManagementContext _context = new BloodTestManagementContext();

        public UserManagement()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var users = _context.Users
                    .Include(u => u.Customer)
                    .Include(u => u.Doctor)
                    .AsNoTracking()
                    .ToList();

                dgUsers.ItemsSource = users;

                // Cập nhật stat cards
                txbAdminCount.Text = users.Count(u => u.Role == "ADMIN").ToString();
                txbDoctorCount.Text = users.Count(u => u.Role == "DOCTOR").ToString();
                txbCustomerCount.Text = users.Count(u => u.Role == "CUSTOMER").ToString();
                txbTotalCount.Text = users.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string role = (cbRole.SelectedItem as ComboBoxItem)?.Content.ToString();
                string password = string.IsNullOrEmpty(txtPassword.Password) ? "1" : txtPassword.Password;

                if (string.IsNullOrEmpty(username) || role == null)
                {
                    MessageBox.Show("Vui lòng nhập Username và chọn Role!");
                    return;
                }

                // Kiểm tra trùng tên trước khi thêm
                if (_context.Users.Any(u => u.Username == username))
                {
                    MessageBox.Show("Tên đăng nhập này đã tồn tại!");
                    return;
                }

                var newUser = new User
                {
                    Username = username,
                    PasswordHash = password,
                    Role = role,
                    CreatedAt = DateTime.Now
                };

                if (role == "CUSTOMER")
                {
                    newUser.Customer = new Entities.Customer { FullName = txtFullName.Text, Phone = txtSubInfo.Text };
                }
                else if (role == "DOCTOR")
                {
                    newUser.Doctor = new Entities.Doctor { FullName = txtFullName.Text, Specialty = txtSubInfo.Text };
                }

                _context.Users.Add(newUser);
                _context.SaveChanges();

                LoadData();
                ClearFields();
                MessageBox.Show($"Thêm thành công! Mật khẩu là: {password}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm mới: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is User selected)
            {
                try
                {
                    var userInDb = _context.Users
                        .Include(u => u.Customer)
                        .Include(u => u.Doctor)
                        .FirstOrDefault(u => u.Id == selected.Id);

                    if (userInDb != null)
                    {
                        string newUsername = txtUsername.Text.Trim();
                        string role = (cbRole.SelectedItem as ComboBoxItem)?.Content.ToString();

                        // Kiểm tra trùng tên nếu thay đổi username
                        if (userInDb.Username != newUsername && _context.Users.Any(u => u.Username == newUsername))
                        {
                            MessageBox.Show("Tên đăng nhập mới bị trùng!");
                            return;
                        }

                        userInDb.Username = newUsername;
                        userInDb.Role = role;

                        // Cập nhật mật khẩu nếu Admin có nhập mới vào ô PasswordBox
                        if (!string.IsNullOrEmpty(txtPassword.Password))
                        {
                            userInDb.PasswordHash = txtPassword.Password;
                        }

                        if (role == "CUSTOMER")
                        {
                            if (userInDb.Customer == null) userInDb.Customer = new Entities.Customer();
                            userInDb.Customer.FullName = txtFullName.Text;
                            userInDb.Customer.Phone = txtSubInfo.Text;
                        }
                        else if (role == "DOCTOR")
                        {
                            if (userInDb.Doctor == null) userInDb.Doctor = new Entities.Doctor();
                            userInDb.Doctor.FullName = txtFullName.Text;
                            userInDb.Doctor.Specialty = txtSubInfo.Text;
                        }
                        
                        _context.SaveChanges();
                        LoadData();
                        MessageBox.Show("Cập nhật thành công!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi cập nhật: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void btnClose_Click(object sender, RoutedEventArgs e) => Close();
        private void btnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is User selected)
            {
                var res = MessageBox.Show("Xóa tài khoản sẽ xóa sạch dữ liệu liên quan. Ok?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res == MessageBoxResult.Yes)
                {
                    try
                    {
                        var userInDb = _context.Users
                            .Include(u => u.Customer)
                            .Include(u => u.Doctor)
                            .FirstOrDefault(u => u.Id == selected.Id);

                        if (userInDb != null)
                        {
                            if (userInDb.Customer != null) _context.Customers.Remove(userInDb.Customer);
                            if (userInDb.Doctor != null) _context.Doctors.Remove(userInDb.Doctor);
                            _context.Users.Remove(userInDb);
                            _context.SaveChanges();
                            LoadData();
                            ClearFields();
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Không thể xóa do User đã có dữ liệu giao dịch (Lịch hẹn/Xét nghiệm).");
                    }
                }
            }
        }

        private void dgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgUsers.SelectedItem is User s)
            {
                txtUsername.Text = s.Username;
                txtPassword.Password = ""; // Không hiển thị ngược Pass để bảo mật

                foreach (ComboBoxItem item in cbRole.Items)
                {
                    if (item.Content.ToString() == s.Role) { cbRole.SelectedItem = item; break; }
                }

                if (s.Role == "CUSTOMER" && s.Customer != null)
                {
                    txtFullName.Text = s.Customer.FullName;
                    txtSubInfo.Text = s.Customer.Phone;
                }
                else if (s.Role == "DOCTOR" && s.Doctor != null)
                {
                    txtFullName.Text = s.Doctor.FullName;
                    txtSubInfo.Text = s.Doctor.Specialty;
                }
                else
                {
                    txtFullName.Text = ""; txtSubInfo.Text = "";
                }
            }
        }

        private void cbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbRole.SelectedItem is ComboBoxItem item)
            {
                string r = item.Content.ToString();
                lblSubInfo.Text = r == "DOCTOR" ? "CHUYÊN KHOA" : (r == "CUSTOMER" ? "SỐ ĐIỆN THOẠI" : "THÔNG TIN THÊM");
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e) => ClearFields();

        private void ClearFields()
        {
            txtUsername.Text = "";
            txtPassword.Password = "";
            txtFullName.Text = "";
            txtSubInfo.Text = "";
            cbRole.SelectedIndex = -1;
            dgUsers.SelectedItem = null;
        }
    }
}