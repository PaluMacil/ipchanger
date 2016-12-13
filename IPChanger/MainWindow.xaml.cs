using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IPChanger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<InterfaceInformation> interfaces = new List<InterfaceInformation>();
        List<SavedInterface> savedInterfaces = new List<SavedInterface>();

        NetworkInterface[] networkInterfaces;

        public MainWindow()
        {
            InitializeComponent();
            interfaces = InterfaceInformation.Deserialize();
            savedInterfaces = SavedInterface.Deserialize();

            updateAllInterfaces();
            listInterfaces.ItemsSource = interfaces;
           
            networkInterfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = 1000;
            t.Elapsed += delegate
            {
                IPConfig.SetInterfaceState(interfaces);
            };
            t.Start();
        }

        void inf_CablePluggedIn(object sender, System.EventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                NetworkAutoplay autoPlay = new NetworkAutoplay((InterfaceInformation)sender);
                autoPlay.InterfaceChanged += delegate { updateAllInterfaces(); };
            }), System.Windows.Threading.DispatcherPriority.Render);
        }

        private void listInterfaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateDataContext();
            updateSavedInterfaces();
        }

        private void btnAddSavedInterface_Click(object sender, RoutedEventArgs e)
        {
            if (listInterfaces.SelectedValue != null)
            {
                wndAddInterface addInterface = new wndAddInterface(listInterfaces.SelectedValue.ToString());
                if(addInterface.ShowDialog() == true)
                {
                    savedInterfaces.Add(addInterface.SavedInterface);
                    SavedInterface.Serialize(savedInterfaces);
                    updateSavedInterfaces();
                }
            }
        }

        private void btnRemoveSavedInterface_Click(object sender, RoutedEventArgs e)
        {
            if(listSavedInterfaces.SelectedValue != null)
            {
                savedInterfaces.Remove((SavedInterface)listSavedInterfaces.SelectedValue);
                SavedInterface.Serialize(savedInterfaces);
                updateSavedInterfaces();
            }
        }

        private void btnChangeAddress_Click(object sender, RoutedEventArgs e)
        {
            if (listSavedInterfaces.SelectedValue != null)
            {
                bool addressChanged = Netsh.SetInterface((SavedInterface)listSavedInterfaces.SelectedValue);

                if(addressChanged)
                {
                    updateAllInterfaces();
                    updateDataContext();
                }
            }
        }

        private void btnSetDefaultInterface_Click(object sender, RoutedEventArgs e)
        {
            SavedInterface newInt = SavedInterface.ConvertInterfacetoSavedInterface((InterfaceInformation)listInterfaces.SelectedValue, "Default");
            
            //if a previous default interface existed, remove it.
            SavedInterface defaultInterface = savedInterfaces.Find(si => si.Name.Equals(((InterfaceInformation)listInterfaces.SelectedValue).Name) && si.SavedInterfaceName.Equals("Default"));
            if(defaultInterface != null)
            {
                savedInterfaces.Remove(defaultInterface);
            }

            savedInterfaces.Add(newInt);
            SavedInterface.Serialize(savedInterfaces);
            updateSavedInterfaces();
        }

        private void updateAllInterfaces()
        {
            foreach (InterfaceInformation inf in interfaces)
            {
                inf.CablePluggedIn -= inf_CablePluggedIn;
            }
            interfaces = Netsh.GetAllInterfaceInformation(interfaces);
            foreach (InterfaceInformation inf in interfaces)
            {
                inf.CablePluggedIn += inf_CablePluggedIn;
            }
            updateDataContext();
            InterfaceInformation.Serialize(interfaces);
        }

        private void updateDataContext()
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (listInterfaces.SelectedValue != null)
                {
                    this.DataContext = interfaces.Find(f => f.Name.Equals(((InterfaceInformation)listInterfaces.SelectedValue).Name));
                }
            }));
        }

        private void updateSavedInterfaces()
        {
            if (listInterfaces.SelectedValue != null)
            {
                listSavedInterfaces.ItemsSource = savedInterfaces.FindAll(si => si.Name.Equals(((InterfaceInformation)listInterfaces.SelectedValue).Name));
            }
        }

        private void listSavedInterfaces_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listSavedInterfaces.SelectedValue != null)
            {
                bool addressChanged = Netsh.SetInterface((SavedInterface)listSavedInterfaces.SelectedValue);

                if (addressChanged)
                {
                    updateAllInterfaces();
                }
            }
        }

        private void chkEnableAutoPlay_Checked(object sender, RoutedEventArgs e)
        {
            ((InterfaceInformation)this.DataContext).EnableAutoPlay = true;
        }
    }
}
