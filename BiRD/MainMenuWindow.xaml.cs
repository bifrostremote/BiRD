using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using BifrostApi.Models;
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
            InitializeComponent();
        }



        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (check_usetestserver.IsChecked == true)
                _api.UseTestServer();
            else
                _api.UseLiveServer();

            string username = txtbox_username.Text;
            string password = txtbox_password.Text;


            string dirtyResult = _api.Login(username, password);
            _userUid = dirtyResult.Replace("\"", "");

            if (_userUid != "")
            {

                // Enable BiRD pages
                ClientPage.IsEnabled = true;
                SupportPage.IsEnabled = true;
                EnrollMachinePage.IsEnabled = true;

                // Set index to the next index in the pages
                tbctrl_pages.SelectedIndex = tbctrl_pages.SelectedIndex + 1;
                LoginPage.Visibility = Visibility.Collapsed;
            }
            else
            {
                lbl_incorrectlogin.Visibility = Visibility.Visible;
            }
        }

        private void Generatetoken_click(object sender, RoutedEventArgs e)
        {
            string token = _api.GenerateWordToken(new BifrostApi.Models.DTO.TokenPairDTO
            {
                MachineUid = new Guid("2d5178bc-5483-40bc-a5e5-7ea3a90b218b"),
                SecurityLevel = 4
            });
        }

        private void ClientPage_LoadMachine(object sender, RoutedEventArgs e)
        {
            List<Machine> machines = _api.GetMachines(new Guid(_userUid));

            _availableMachines = new ObservableCollection<Machine>(machines);
        }
    }
}
