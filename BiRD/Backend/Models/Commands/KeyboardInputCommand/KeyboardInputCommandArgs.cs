using BifrostRemoteDesktop.Common.Models.Commands;

namespace BiRD.Backend.Models.Commands
{
    public class KeyboardInputCommandArgs : RemoteControlCommandArgs
    {

        public const int KEY_STATE_CODE_UP = 0;
        public const int KEY_STATE_CODE_DOWN = 1;

        public int VKeyCode { get; set; }
        public int KeyStateCode { get; set; }
    }
}