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

namespace IPChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<InterfaceInformation> interfaces = new List<InterfaceInformation>();

        public MainWindow()
        {
            InitializeComponent();
            interfaces = Netsh.GetAllInterfaceInformation();
            listInterfaces.ItemsSource = interfaces;
        }

        private void listInterfaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.DataContext = interfaces.Find(f => f.Name.Equals(((InterfaceInformation)listInterfaces.SelectedValue).Name));
        }
    }
}
