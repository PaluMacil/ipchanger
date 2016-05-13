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

namespace IPChanger
{
    /// <summary>
    /// Interaction logic for wndAddInterface.xaml
    /// </summary>
    public partial class wndAddInterface : Window
    {

        public SavedInterface SavedInterface { get; set; }

        public wndAddInterface(string interfaceName)
        {
            InitializeComponent();

            txtInterfaceName.Text = interfaceName;
        }

        private void cbDHCP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbDHCP.SelectedValue == "Yes")
            {
                //IP, mask, and gateway don't really matter. Should we clear and disable them?
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SavedInterface newInterface = new SavedInterface();
            newInterface.Name = txtInterfaceName.Text;
            newInterface.SavedInterfaceName = txtSavedName.Text;
            newInterface.IPAddress = txtIPAddress.Text;
            newInterface.IPMask = txtMask.Text;
            newInterface.Gateway = txtGateway.Text;
            newInterface.IsDHCP = cbDHCP.SelectedValue == "Yes";

            SavedInterface = newInterface;
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
