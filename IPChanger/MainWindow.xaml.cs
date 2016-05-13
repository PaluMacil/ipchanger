using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace IPChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<InterfaceInformation> interfaces = new List<InterfaceInformation>();

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