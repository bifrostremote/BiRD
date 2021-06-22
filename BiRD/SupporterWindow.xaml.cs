using BifrostRemoteDesktop.Common.Enums;
using BifrostRemoteDesktop.Common.Models.Commands;
using BifrostRemoteDesktop.Common.Network;
using BifrostRemoteDesktop.WPF.Backend.Models.Commands;
using BiRD.Backend.Models.Commands;
using BiRD.Backend.Models.Commands.MouseActionCommand;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace BiRD
{
    /// <summary>
    /// Interaction logic for SupporterWindow.xaml
    /// </summary>
    public partial class SupporterWindow : Window
    {
        CommandTransmitter commandTransmitter = new CommandTransmitter();
        string ClientIP = "127.0.0.1";
        Thread mainThread;
        UdpClient udpServer;
        bool shouldRecieve = true;

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        public SupporterWindow(string ip, string localhost)
        {
            InitializeComponent();
            ClientIP = ip;
            commandTransmitter.Connect(ClientIP);
            mainThread = new Thread(new ThreadStart(MainProgram));
            mainThread.IsBackground = true;
            mainThread.Start();

            // Send connect request via tcp
            commandTransmitter.SendCommand(CommandType.ConnectionRequest,
                new ConnectionRequestCommandArgs()
                {
                    IP = localhost,
                    StreamRunning = true
                });
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            shouldRecieve = false;
            mainThread.Interrupt();
            udpServer.Client.Shutdown(SocketShutdown.Receive);
            udpServer.Client.Close();
            udpServer.Close();
            // Send connect request via tcp
            commandTransmitter.SendCommand(CommandType.ConnectionRequest,
                new ConnectionRequestCommandArgs()
                {
                    IP = "",
                    StreamRunning = false
                });
            //Environment.Exit(Environment.ExitCode); // Prevent memory leak
            //System.Windows.Application.Current.Shutdown(); // Not sure if needed
        }

        void MainProgram()
        {
            //Console.WriteLine("Hello World!");
            udpServer = new UdpClient(11000);
            while (shouldRecieve)
            {
                try
                {
                    var remoteEP = new IPEndPoint(IPAddress.Any, 11000);
                    if (shouldRecieve)
                    {
                        byte[] data = udpServer.Receive(ref remoteEP); // listen on port 11000
                                                                       //Console.WriteLine("receive data from " + remoteEP.ToString());
                                                                       //Console.WriteLine(ByteArrayToString(data));
                        //HandleData(data);
                        // NOTE: Possible speed increse.
                        Task.Run(() => HandleData(data));
                        //udpServer.Send(new byte[] { 1 }, 1, remoteEP); // reply back, reqires listener on other side.
                    }

                } catch (SocketException ex)
                {
                    var errorcode = ex.ErrorCode;
                }
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        int headerLen = 5; // 0 = type, 1 & 2 = index, 3

        FrameData bufferFrame = new FrameData();
        FrameData handleFrame = new FrameData();
        Thread frameHandle;

        public void HandleData(byte[] data)
        {
            int headerInfo = data[0];

            // If new frame is recieved, then handle the data.
            string identifier = BitConverter.ToString(new byte[] { data[1], data[2] }).Replace("-", string.Empty);
            if (headerInfo == 100)
            {
                if (bufferFrame != null)
                {
                    // Check if the handle thread isn't running.
                    if (frameHandle == null || !frameHandle.IsAlive)
                    {
                        // If no id is here, then just return.
                        if (!String.IsNullOrEmpty(bufferFrame.ID))
                        {
                            handleFrame = (FrameData)bufferFrame.Clone();
                            frameHandle = new Thread(new ThreadStart(HandleFrame));
                            frameHandle.Start();
                        }
                    }
                }
            }
            else if (headerInfo == 101)
            {

                bufferFrame = new FrameData(data);
            }
            else
            {
                if (bufferFrame.ID == identifier)
                    bufferFrame.AddChunk(data);
            }
        }

        byte[] previousFrame;
        public byte[] CompareFrame(byte[] newFrame)
        {
            byte[] returnFrame = new byte[newFrame.Length];

            if (previousFrame != null && previousFrame.Length == newFrame.Length)
            {
                Parallel.ForEach(newFrame, (frame, fx, index) =>
                {
                    if (frame == 0)
                        returnFrame[index] = previousFrame[index];
                    else if (frame == 1)
                        returnFrame[index] = 0;
                    else
                        returnFrame[index] = frame;
                });
            }
            else
                returnFrame = newFrame;

            previousFrame = returnFrame;

            return returnFrame;
        }

        public void SendReq(byte[] data)
        {

        }

        public void HandleFrame()
        {
            byte[] bitmapRawImage = handleFrame.BuildBitmapRaw();
            bitmapRawImage = CompareFrame(bitmapRawImage);
            // Return if no image is found. Or the first 10 bytes is empty.
            if (bitmapRawImage.All(singleByte => singleByte == 0 || singleByte == 1) || bitmapRawImage.Take(10).All(singleByte => singleByte == 0))
                return;

            UpdateImage(bitmapRawImage);
        }


        public static Bitmap ByteToImage(byte[] bitmap)
        {
            using (var ms = new MemoryStream(bitmap))
            {
                ms.Seek(0, SeekOrigin.Begin);
                return new Bitmap(ms);
            }
        }

        Object lockObj = new Object();
        private void UpdateImage(byte[] bitmapImage)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                using (var ms = new MemoryStream(bitmapImage))
                {
                    Bitmap bitmap;
                    ms.Seek(0, SeekOrigin.Begin);
                    try
                    {
                        bitmap = new Bitmap(ms);
                    }
                    catch (Exception)
                    {
                        return;
                    }

                    // Display crap.
                    IntPtr handle = IntPtr.Zero;
                    try
                    {
                        handle = bitmap.GetHbitmap();
                        ImageContainer.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                    }
                    catch (Exception) { }
                    finally
                    {
                        DeleteObject(handle);
                    }
                }
            });
        }

        private void ImageContainer_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition((IInputElement)sender);
            commandTransmitter.SendCommand(CommandType.MovePointerPercentage, new MovePointerPercentageCommandArgs()
            {
                PercentageX = pos.X / ImageContainer.ActualWidth,
                PercentageY = pos.Y / ImageContainer.ActualHeight
            });
        }

        private void ImageContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            commandTransmitter.SendCommand(CommandType.MouseAction,
                new MouseActionCommandArgs()
                {
                    ActionType = MouseActionType.PRESS_LEFT_BTN
                });
        }

        private void ImageContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            commandTransmitter.SendCommand(CommandType.MouseAction,
                new MouseActionCommandArgs()
                {
                    ActionType = MouseActionType.RELEASE_LEFT_BTN
                });
        }

        private void ImageContainer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            commandTransmitter.SendCommand(CommandType.MouseAction,
                new MouseActionCommandArgs()
                {
                    ActionType = MouseActionType.PRESS_RIGHT_BTN
                });
        }

        private void ImageContainer_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            commandTransmitter.SendCommand(CommandType.MouseAction,
                new MouseActionCommandArgs()
                {
                    ActionType = MouseActionType.RELEASE_RIGHT_BTN
                });
        }

        private void ImageContainer_KeyDown(object sender, KeyEventArgs e)
        {
            commandTransmitter.SendCommand(CommandType.KeyboardInputCommand,
                new KeyboardInputCommandArgs()
                {
                    KeyStateCode = KeyboardInputCommandArgs.KEY_STATE_CODE_DOWN,
                    VKeyCode = KeyInterop.VirtualKeyFromKey(e.Key)
                });
        }

        private void ImageContainer_KeyUp(object sender, KeyEventArgs e)
        {
            commandTransmitter.SendCommand(CommandType.KeyboardInputCommand,
                new KeyboardInputCommandArgs()
                {
                    KeyStateCode = KeyboardInputCommandArgs.KEY_STATE_CODE_UP,
                    VKeyCode = KeyInterop.VirtualKeyFromKey(e.Key)
                });
        }
    }
}
