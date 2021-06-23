using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
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
using BifrostApi.Models;
using BifrostApi.Models.DTO;
using BiRD.Backend.API;

namespace BiRD
{
    /// <summary>
    /// Interaction logic for EnrollPage.xaml
    /// </summary>
    public partial class MainMenuWindow : Window
    {
        private readonly BifrostAPI _api;
        private string _userUid;
        private ObservableCollection<Machine> _availableMachines;
        public ObservableCollection<Machine> AvailableMachines
        {
            get
            {
                return _availableMachines;
            }
            set
            {
                _availableMachines = value;
            }
        }

        private Machine _selectedMachine;
        public Machine SelectedMachine
        {
            get
            {
                return _selectedMachine;
            }
            set
            {
                _selectedMachine = value;
            }
        }

        public MainMenuWindow()
        {
            _api = BifrostAPI.GetInstance();
            this.DataContext = this;
            LoadMachines += this.ClientPage_LoadMachine;

            InitializeComponent();
        }

        public event EventHandler<RoutedEventArgs> LoadMachines;

        private void ClientPage_LoadMachine(object sender, RoutedEventArgs e)
        {
            List<Machine> machines = _api.GetMachines(new Guid(_userUid));

            cmb_machines.ItemsSource = machines;

            var addresses = GetIpAddresses();

            cmb_addresses.ItemsSource = addresses;
            cmb_addresses.SelectedIndex = 0;

            cmb_supporter_addresses.ItemsSource = addresses;
            cmb_supporter_addresses.SelectedIndex = 0;
           

            //_availableMachines = new ObservableCollection<Machine>(machines);
        }

        private static ObservableCollection<string> GetIpAddresses()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            ObservableCollection<string> addresses = new ObservableCollection<string>();

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    addresses.Add(ip.ToString());
                }
            }

            return addresses;
        }

        #region Login
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (check_usetestserver.IsChecked == true)
                _api.UseTestServer();
            else
                _api.UseLiveServer();

            // Gather credentials
            string username = txtbox_username.Text;
            string password = txtbox_password.Password;

            // User login
            string dirtyResult = _api.Login(username, password);
            _userUid = dirtyResult.Replace("\"", "");

            if (_userUid != "")
            {
                // Enable BiRD pages
                ClientPage.IsEnabled = true;
                SupportPage.IsEnabled = true;
                EnrollMachinePage.IsEnabled = true;

                EventHandler<RoutedEventArgs> handler = LoadMachines;
                if (handler != null)
                {
                    handler(this, null);
                }

                // Set index to the next index in the pages
                tbctrl_pages.SelectedIndex = tbctrl_pages.SelectedIndex + 1;
                LoginPage.Visibility = Visibility.Collapsed;
            }
            else
            {
                lbl_incorrectlogin.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region Client
        private void Generatetoken_click(object sender, RoutedEventArgs e)
        {
            Machine machine = (Machine)cmb_machines.SelectedItem;

            string token = _api.GenerateWordToken(new BifrostApi.Models.DTO.TokenPairDTO
            {
                MachineUid = machine.Uid,
                SecurityLevel = 4
            });
            lbl_Token.Content = token;
        }

        private void btn_OpenRemote_Click(object sender, RoutedEventArgs e)
        {
            ClientWindow clientWindow = new ClientWindow();
            clientWindow.Show();
        }

        private void lbl_Token_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(((Label)sender).Content.ToString());
        }
        #endregion

        #region Supporter
        private void btn_connect_click(object sender, RoutedEventArgs e)
        {
            string ip = _api.GetMachineIp(txtbox_SupporterToken.Text);

            if (ip == "")
            {
                // TODO: Add error message.
                return;
            }

            SupporterWindow supporterWindow = new SupporterWindow(ip, cmb_supporter_addresses.SelectedItem.ToString());
            supporterWindow.Show();
        }
        #endregion

        #region Enroll
        private void btn_Enroll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string machineName = Environment.MachineName;
                string ip = (string)cmb_addresses.SelectedItem;

                var machine = new MachineCreateDTO
                {
                    IPAddress = ip,
                    Name = machineName,
                    UserUid = new Guid(_userUid)
                };

                Guid newMachine = _api.EnrollMachine(machine);

                ClientPage_LoadMachine(this, null);

            }
            catch (ArgumentException ex)
            {

            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
