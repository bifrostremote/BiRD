using BifrostRemoteDesktop.Common.Models.Commands;
using BifrostRemoteDesktop.Common.SystemControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiRD.Backend.Models.Commands
{
    public class KeyboardInputCommand : RemoteControlCommand<KeyboardInputCommandArgs>
    {

        public KeyboardInputCommand(
            ISystemController systemController, KeyboardInputCommandArgs args) : base(systemController, args)
        {

        }

        public override void Execute()
        {
            switch (Args.KeyStateCode)
            {
                case KeyboardInputCommandArgs.KEY_STATE_CODE_DOWN:
                    SystemController.Keyboard.PressKey(Args.VKeyCode);
                    break;
                case KeyboardInputCommandArgs.KEY_STATE_CODE_UP:
                    SystemController.Keyboard.ReleaseKey(Args.VKeyCode);
                    break;
                default:
                    throw new ArgumentException("Unexpected key state code.");
            }
        }
    }
}
