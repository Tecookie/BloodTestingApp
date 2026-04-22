using System;
using System.Windows;
using System.Windows.Controls;
using BloodTestingApp.Entities; // Hãy đảm bảo tên này đúng với thư mục Entities của bạn

namespace BloodTestingApp.Pages
{
    public partial class CustomerPage : Page
    {
        public CustomerPage()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            // 1. Kiểm tra xem người dùng đã chọn ngày chưa
            if (dpDate.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày đặt lịch!");
                return;
            }

            DateTime selectedDate = dpDate.SelectedDate.Value;

            // 2. Kiểm tra quy định: Chỉ từ Thứ 2 đến Thứ 7
            if (selectedDate.DayOfWeek == DayOfWeek.Sunday)
            {
                MessageBox.Show("Phòng khám không làm việc vào Chủ Nhật. Vui lòng chọn ngày khác!");
                return;
            }

            // 3. Thực hiện lưu vào Database
            try
            {
                using (var context = new BloodTestManagementContext())
                {
                    var newAppointment = new Appointment
                    {
                        CustomerId = 1, // Tạm thời để mặc định là 1 (Khách hàng đầu tiên)
                        AppointmentDate = selectedDate,
                        Status = "Pending", // Trạng thái chờ bác sĩ nhận
                        // Note = txtNote.Text // Bỏ comment nếu trong bảng Appointment có cột Note
                    };

                    context.Appointments.Add(newAppointment);
                    context.SaveChanges(); // Lệnh quan trọng nhất để đẩy dữ liệu lên SQL

                    MessageBox.Show("Gửi yêu cầu đặt lịch thành công! Bác sĩ sẽ sớm tiếp nhận.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message);
            }
        }
    }
}