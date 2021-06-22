using BifrostRemoteDesktop.Common.Network;
using BifrostRemoteDesktop.Common.SystemControllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BiRD
{
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        #region Mouse stuff
        //// Move mouse stuff.
        CommandReceiver commandReceiver = new CommandReceiver(new WindowsSystemController());
        #endregion
        public string ClientIP = "127.0.0.1";
        int QualityPercetn = 15;
        ClientWindow _thisWindow;

        Thread record;
        public ClientWindow()
        {
            InitializeComponent();
            commandReceiver.ParseWindow(this);
            commandReceiver.Start();
            _thisWindow = this;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr onj);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(CommandReceiver onj);

        public void CloseWinow()
        {
            loop = false;
            commandReceiver.Stop();
            DeleteObject(commandReceiver);
            Environment.Exit(Environment.ExitCode);
        }

        #region Compare frame code
        byte[] previousFrame;
        public byte[] CompareFrame(byte[] newFrame)
        {
            byte[] returnFrame = new byte[newFrame.Length];

            if (previousFrame != null && previousFrame.Length == newFrame.Length)
            {
                Parallel.For(0, newFrame.Length, index =>
                //Parallel.ForEach(newFrame, (frame, fx, index) =>
                {
                    if (newFrame[index] == 0)
                        newFrame[index] = 1;

                    if (previousFrame[index] == newFrame[index])
                        returnFrame[index] = 0;
                    else
                        returnFrame[index] = newFrame[index];
                });
            }
            else
                returnFrame = newFrame;

            previousFrame = newFrame;

            return returnFrame;
        }
        #endregion

        Random rnd = new Random();
        #region Send data
        public void SendByteStrean(byte[][] chunks, int dataLen, int imageWidth, int imageHeight, string ip, int port)
        {
            IPEndPoint ep;
            try
            {
                // Create the endpoint the the udp connection will use.
                ep = new IPEndPoint(IPAddress.Parse(ip), port);
            }
            catch (Exception) { return; }

            // Create a new socket.
            Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // Generate id for package.
            byte[] id = new byte[2];
            rnd.NextBytes(id);
            // Send header to prepare for new message.
            byte[] header = new byte[]
            {
                101, // Start byte

                id[0], // ID_1
                id[1], // ID_2

                Convert.ToByte(chunks.Length % 255), // List count single
                Convert.ToByte((int)(chunks.Length / 255)), // List count tenths
                
                Convert.ToByte(dataLen % 255), // Total length single
                Convert.ToByte((int)((dataLen / 255) % 255)), // Total length tenths
                Convert.ToByte((int)((dataLen / 255) / 255)), // Total length tenths

                Convert.ToByte(imageWidth % 255), // Canvas Width single
                Convert.ToByte((int)(imageWidth / 255)), // Width tenths

                Convert.ToByte(imageHeight % 255), // Canvas Height single
                Convert.ToByte((int)(imageHeight / 255)) // Height tenths
            };

            // Send data to the endpoint using the socket connection.
            sending_socket.SendTo(header, ep);
            //Thread.Sleep(1);

            for (int i = 0; i < chunks.Length; i++)
            {
                // Break if all chunks are empty.
                if (chunks[i] == null || chunks[i].All(x => x == 0))
                    continue;
                //var id = GetRandomHexNumber(4);
                // Create a custom data packet.
                header = new byte[]
                {
                    0, // Start byte
                    id[0], // ID_1
                    id[1], // ID_2
                    Convert.ToByte(i%255), // Index single
                    Convert.ToByte((int)(i/255)), // Index tenths
                };

                byte[] package = new byte[header.Length + chunks[i].Length];
                header.CopyTo(package, 0);
                chunks[i].CopyTo(package, header.Length);

                // Send data to the endpoint using the socket connection.
                sending_socket.SendTo(package, ep);
                if (i % QualityPercetn == 0)
                    Thread.Sleep(1);
            }

            Thread.Sleep(1);
            // Send end frame
            byte[] dat = new byte[]
            {
                100, // Start byte
                id[0], // ID_1
                id[1], // ID_2
            };

            // Send data to the endpoint using the socket connection.
            sending_socket.SendTo(dat, ep);
        }
        #endregion

        public static byte[] ImageToByte(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public bool loop;

        public void RecordScreen()
        {
            while (loop)
            {
                Bitmap bitmap;

                bitmap = new Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                }

                // Handle code.
                byte[] bitByte = ImageToByte(bitmap);
                bitByte = CompareFrame(bitByte);

                int sendSize = 500;
                #region Split function.
                int size = bitByte.Length / sendSize;
                byte[][] chunks = new byte[size + 1][];
                //for (int i = 0; i < size; i++)
                Parallel.For(0, size, (i) =>
                {
                    byte[] tmpArr = new byte[sendSize];
                    Array.Copy(bitByte, i * sendSize, tmpArr, 0, sendSize);
                    chunks[i] = tmpArr;
                });
                // Take last item.
                byte[] tmpArr2 = new byte[bitByte.Length % sendSize];
                Array.Copy(bitByte, size * sendSize, tmpArr2, 0, bitByte.Length % sendSize);
                chunks[size] = tmpArr2;
                #endregion

                // Send data
                //SendByteStrean(myStream.GetBuffer(), ImageSize, "127.0.0.1", 11000); //"10.142.105.45"
                SendByteStrean(chunks, bitByte.Length, bitmap.Width, bitmap.Height, ClientIP, 11000); //"10.142.112.247"


                UpdateImage(bitmap);
            }
        }

        public void Start_Streaming()
        {
            loop = true;
            record = new Thread(RecordScreen);
            record.IsBackground = true;
            record.Start();
        }

        public void Stop_Streaming()
        {
            loop = false;
            commandReceiver.Stop();
        }

        private void UpdateImage(Bitmap bitmap)
        {
            Dispatcher.Invoke(() => {

                // Display crap.
                IntPtr handle = IntPtr.Zero;
                try
                {
                    handle = bitmap.GetHbitmap();
                    ImageControl.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
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
