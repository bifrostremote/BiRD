using BifrostRemoteDesktop.Common.Models.Commands;
using BifrostRemoteDesktop.Common.SystemControllers;

namespace BifrostRemoteDesktop.WPF.Backend.Models.Commands.MovePointerPercentageCommand
{

    public class MovePointerPercentCommand : RemoteControlCommand<MovePointerPercentageCommandArgs>
    {
        public MovePointerPercentCommand(ISystemController systemController, MovePointerPercentageCommandArgs args) : base(systemController, args) { }

        public override void Execute()
        {
            SystemController.Mouse.SetCursorPositionPercentage(
                percentageX: Args.PercentageX,
                percentageY: Args.PercentageY);
        }
    }
}
