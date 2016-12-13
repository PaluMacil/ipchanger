using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for NetworkAutoplay.xaml
    /// </summary>
    public partial class NetworkAutoplay : Window
    {
        private InterfaceInformation inf;
        private List<SavedInterface> savedInterfaces;
        public event EventHandler InterfaceChanged = delegate { };


        public NetworkAutoplay(InterfaceInformation interfaceInfo)
        {
            savedInterfaces = SavedInterface.Deserialize();
            inf = interfaceInfo;
            InitializeComponent();
            lblNetworkAdapterName.Content = inf.Name;
            lblCurrentIPAddress.Content = inf.IPAddress;
            fillInSavedInterfaceList();
            this.Topmost = true;
            this.Show();
            this.MouseDown += NetworkAutoplay_MouseDown;
        }

        void NetworkAutoplay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void fillInSavedInterfaceList()
        {
            lstSavedInterfaces.ItemsSource = savedInterfaces.FindAll(si => si.Name.Equals(inf.Name));
            lstSavedInterfaces.DataContext = savedInterfaces;
        }

        volatile bool addressChanged = false;
        object lockObject = new object();
        private void lstSavedInterfaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstSavedInterfaces.SelectedValue != null)
            {
                lock (lockObject)
                {
                    addressChanged = false;
                }
                lblSettingIP.Visibility = System.Windows.Visibility.Visible;
                lstSavedInterfaces.IsEnabled = false;
                new Thread((object threadInfo) =>
                {
                    ThreadInfo info = (ThreadInfo)threadInfo;
                    lock (lockObject)
                    {
                        addressChanged = Netsh.SetInterface((SavedInterface)info.savedInterface);
                    }
                    ((EventHandler)info.callBack)(null, null);
                }).Start(new ThreadInfo() { callBack = SetAddressCallback, savedInterface = (SavedInterface)lstSavedInterfaces.SelectedValue });
            }
        }

        private void SetAddressCallback(object sender, EventArgs e)
        {
            if (addressChanged)
            {
                InterfaceChanged(null, null);
                Dispatcher.Invoke(new Action(delegate
                {
                    lblSettingIP.Content = "Success!";
                    System.Timers.Timer t = new System.Timers.Timer();
                    t.Interval = 1500;
                    t.Elapsed += delegate
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            t.Stop();
                            this.Close();
                        }));
                    };
                    t.Start();
                }));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    class ThreadInfo
    {
        public EventHandler callBack;
        public SavedInterface savedInterface;
    }
}