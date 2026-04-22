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
using System.Windows.Shapes;

namespace BloodTestingApp.Pages.Doctor
{
    /// <summary>
    /// Interaction logic for DoctorMainWindow.xaml
    /// </summary>
    public partial class DoctorMainWindow : Window
    {
        public DoctorMainWindow()
        {
            InitializeComponent();
        }
        private void Pending_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DoctorPendingPage());
        }

        private void Assigned_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DoctorAssignedPage());
        }

        private void Result_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DoctorResultPage());
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DoctorHistoryPage());
        }
    }
}
