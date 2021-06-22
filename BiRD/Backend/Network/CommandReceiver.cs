using BifrostRemoteDesktop.Common.Enums;
using BifrostRemoteDesktop.Common.Factories;
using BifrostRemoteDesktop.Common.Models.Commands;
using BifrostRemoteDesktop.Common.SystemControllers;
using BiRD;
using BiRD.Backend.Models.Commands;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BifrostRemoteDesktop.Common.Network
{

    public class CommandReceiver
    {
        private readonly ISystemController _systemController;
        private Thread thread;
        private TcpListener tcpListener;
        public ClientWindow client;


        public CommandReceiver(ISystemController systemController)
        {
            _systemController = systemController;
        }

        ~CommandReceiver()
        {
            Stop();
            tcpListener.Stop();
        }

        public void ParseWindow(ClientWindow cl)
        {
            client = cl;
        }

        public void Start()
        {
            if (thread == null)
            {
                thread = new Thread(Listen);
            }
            if (tcpListener == null)
            {
                tcpListener = new TcpListener(IPAddress.Any, TransmissionContext.INPUT_TCP_PORT);
            }
            if (!thread.IsAlive)
            {
                tcpListener.Start();
                thread.Start(this);
            }
        }

        public void Stop()
        {
            if (thread != null && thread.IsAlive)
            {
                tcpListener.Stop();
                thread.Interrupt();
            }
        }

        private void Listen(object obj)
        {
            if (obj is CommandReceiver commandReceiver)
            {
                //IPAddress localhost = IPAddress.Parse("127.0.0.1");

                byte[] buffer = new byte[TransmissionContext.RECEIVER_BUFFER_SIZE];
                string data = string.Empty;
                while (true)
                {
                    TcpClient receiver = commandReceiver.tcpListener.AcceptTcpClient();
                    NetworkStream stream = receiver.GetStream();
                    int i;

                    while ((i = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        data += Encoding.UTF8.GetString(buffer, 0, i);
                        while (TryFindAndRemoveNextPackage(ref data, out string package))
                        {
                            ICommand command = ParseCommandFromPackage(
                                commandReceiver._systemController, package);
                            // TODO: This is a hack, maby make it right later?
                            if (command is ConnectionRequestCommand)
                                command.Execute(client);
                            else
                                command.Execute();
                        }
                    }

                    receiver.Close();
                }
            }
        }

        /// <summary>
        /// This method will try to find the next package in a data string.
        /// If a package is found it will remove it from the data string and return true.
        /// </summary>
        /// <param name="data">The data string to attempt package extraction on.</param>
        /// <param name="package">The found package. (string.Empty if no package is found.)</param>
        /// <returns>Returns true if a package was found.</returns>
        public static bool TryFindAndRemoveNextPackage(ref string data, out string package)
        {
            int startCharIndex = data.IndexOf(TransmissionContext.START_OF_TEXT_CHAR);
            if (startCharIndex == -1)
            {
                package = string.Empty;
                return false;
            }

            int endCharIndex = data.IndexOf(TransmissionContext.END_OF_TEXT_CHAR, startCharIndex);

            if (endCharIndex == -1)
            {
                package = string.Empty;
                return false;
            }

            int packageSize = endCharIndex - startCharIndex;
            package = data.Substring(startCharIndex + 1, packageSize - 1);
            data = data.Remove(startCharIndex, packageSize);
            return true;
        }

        public static ICommand ParseCommandFromPackage(ISystemController systemController, string package)
        {
            string[] parts = package.Split(TransmissionContext.TEXT_SEGMENTATION_CHAR);

            if (!Enum.TryParse(parts[0], out CommandType commandType))
            {
                throw new InvalidCastException();
            }

            RemoteControlCommandArgs commandArgs = JsonConvert.DeserializeObject<RemoteControlCommandArgs>(parts[1], new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            });
            ICommand command = CommandFactory.CreateCommand(commandType, commandArgs, systemController);
            return command;
        }
    }
}