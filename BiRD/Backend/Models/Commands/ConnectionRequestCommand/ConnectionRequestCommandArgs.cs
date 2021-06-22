using BifrostRemoteDesktop.Common.Models.Commands;

namespace BiRD.Backend.Models.Commands
{
    public class ConnectionRequestCommandArgs : RemoteControlCommandArgs
    {
        public string IP { get; set; }
        public bool StreamRunning { get; set; }
    }
}
