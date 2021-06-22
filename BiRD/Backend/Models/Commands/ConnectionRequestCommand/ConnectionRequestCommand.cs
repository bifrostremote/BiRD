using BifrostRemoteDesktop.Common.Models.Commands;
using BifrostRemoteDesktop.Common.SystemControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiRD.Backend.Models.Commands
{
    public class ConnectionRequestCommand : RemoteControlCommand<ConnectionRequestCommandArgs>
    {
        public ConnectionRequestCommand(
            ISystemController systemController, ConnectionRequestCommandArgs args) : base(systemController, args)
        {

        }

        public override void Execute() {}

        public override void Execute(object obj)
        {
            ((ClientWindow)obj).ClientIP = Args.IP;

            if (Args.StreamRunning)
                ((ClientWindow)obj).Start_Streaming();
            else
                ((ClientWindow)obj).CloseWinow();
        }
    }
}
