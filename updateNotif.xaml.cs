using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
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

namespace itsoutchyCord
{
    /// <summary>
    /// Interaction logic for updateNotif.xaml
    /// </summary>
    public partial class updateNotif : Window
    {
        public updateNotif()
        {
            InitializeComponent();
            verLabel.Content = "Version " + MainWindow.onlineVer + " is available!";
            IsVisibleChanged += UpdateNotif_IsVisibleChanged;
        }

        private async void UpdateNotif_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                await Task.Delay(7000);
                Close();
            }
        }
    }
}
