
namespace BifrostRemoteDesktop.Common.Models.Commands
{
    public class PointerUpdateStateCommandArgs : RemoteControlCommandArgs
    {
        public PointerUpdateStateCommandArgs() { }

        public bool IsLeftPointerButtonPressed { get; set; }
        public bool IsRightPointerButtonPressed { get; set; }
        public bool IsMiddlePointerButtonPressed { get; set; }
    }
}
