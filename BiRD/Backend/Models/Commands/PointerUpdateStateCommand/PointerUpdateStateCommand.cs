using BifrostRemoteDesktop.Common.SystemControllers;
using System;

namespace BifrostRemoteDesktop.Common.Models.Commands
{
    public class PointerUpdateStateCommand : RemoteControlCommand<PointerUpdateStateCommandArgs>
    {

        public PointerUpdateStateCommand(
            ISystemController systemController, PointerUpdateStateCommandArgs args) : base(systemController, args)
        {

        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
