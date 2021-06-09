
using BiRD.Backend.Models.Commands.MouseActionCommand;

namespace BifrostRemoteDesktop.Common.Models.Commands
{
    public class MouseActionCommandArgs : RemoteControlCommandArgs
    {
        public MouseActionType ActionType { get; set; }

        public MouseActionCommandArgs() {}

    }
}
