using BifrostRemoteDesktop.Common.SystemControllers;
using BiRD.Backend.Models.Commands.MouseActionCommand;
using System;

namespace BifrostRemoteDesktop.Common.Models.Commands
{
    public class MouseActionCommand : RemoteControlCommand<MouseActionCommandArgs>
    {

        public MouseActionCommand(
            ISystemController systemController, MouseActionCommandArgs args) : base(systemController, args)
        { }

        public override void Execute()
        {
            switch (Args.ActionType)
            {
                case MouseActionType.PRESS_LEFT_BTN:
                    SystemController.Mouse.PressLeftButton();
                    break;
                case MouseActionType.RELEASE_LEFT_BTN:
                    SystemController.Mouse.ReleaseLeftButton();
                    break;
                case MouseActionType.PRESS_RIGHT_BTN:
                    SystemController.Mouse.PressRightButton();
                    break;
                case MouseActionType.RELEASE_RIGHT_BTN:
                    SystemController.Mouse.ReleaseRightButton();
                    break;
                default:
                    throw new NotImplementedException($"No action is implemented for argument {Args.ActionType} of type {nameof(MouseActionType)}");
            }
        }
    }
}
