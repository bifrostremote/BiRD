using BifrostRemoteDesktop.Common.Network;
using BifrostRemoteDesktop.Common.SystemControllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BiRD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RecorderWindow : Window
    {
        private readonly CommandReceiver commandReceiver;
        private readonly ScreenRecorder recorder;

        public RecorderWindow()
        {
            InitializeComponent();

            commandReceiver = new CommandReceiver(new WindowsSystemController());
            commandReceiver.Start();

            recorder = ScreenRecorderStore.GetRecorder();
            recorder.RemoteIp = IPAddress.Parse("10.142.109.247");
            recorder.FrameCaptured += Recorder_FrameCaptured;
        }

        private void Recorder_FrameCaptured(Bitmap bitmap)
        {
            UpdateImage(bitmap);
        }

        private void OnWindowclose(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode); // Prevent memory leak
                                                    // System.Windows.Application.Current.Shutdown(); // Not sure if needed
        }

        private void Start_Streaming(object sender, RoutedEventArgs e)
        {
            recorder.StartStreaming();
        }

        private void Stop_Streaming(object sender, RoutedEventArgs e)
        {
            recorder.StopStreaming();
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr onj);

        private void UpdateImage(Bitmap bitmap)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {

                // Display crap.
                IntPtr handle = IntPtr.Zero;
                try
                {
                    handle = bitmap.GetHbitmap();
                    ImageControl.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                    //bitmap.Save("C:\\1.jpg"); //saving
                }
                catch (Exception)
                {

                }
                finally
                {
                    DeleteObject(handle);
                }
            });
        }

    }
}
