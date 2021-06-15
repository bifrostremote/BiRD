using BiRD.Backend.API;
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

namespace BiRD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            BifrostAPI api = BifrostAPI.GetInstance();
            api.Login("SuperAdmin", "EQF6cnG-jbVj~.t?@4RNr]A%");

            api.GetMachine(new Guid("b2b985a8-67b5-428e-aace-0b479810f346"));

            InitializeComponent();
        }

        private void Start_Client_Click(object sender, RoutedEventArgs e)
        {
            RecorderWindow recorderWindow = new RecorderWindow();
            recorderWindow.Show();
        }

        private void Start_Support_Click(object sender, RoutedEventArgs e)
        {
            PlayerWindow playerWindow = new PlayerWindow();
            playerWindow.Show();
        }
    }
}
