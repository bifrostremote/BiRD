using BifrostRemoteDesktop.Common.SystemControllers;

namespace BifrostRemoteDesktop.Common.Models.Commands
{

    public class MovePointerCommand : RemoteControlCommand<MovePointerCommandArgs>
    {
        public MovePointerCommand(ISystemController systemController, MovePointerCommandArgs args) : base(systemController, args) { }

        public override void Execute()
        {
            SystemController.Mouse.SetCursorPosition(
                x: Args.TargetX,
                y: Args.TargetY);
        }
    }
}
