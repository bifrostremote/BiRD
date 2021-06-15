using BifrostRemoteDesktop.Common.Models.Commands;

namespace BiRD.Backend.Models.Commands
{
    public class KeyboardInputCommandArgs : RemoteControlCommandArgs
    {
        public int VKeyCode { get; set; }
    }
}