using BifrostRemoteDesktop.Common.Models.Commands;
using BifrostRemoteDesktop.Common.SystemControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BiRD.Backend.Models.Commands
{
    public class StartStreamingCommand : RemoteControlCommand<StartStreamingCommandArgs>
    {
        
        public StartStreamingCommand(ISystemController systemController, StartStreamingCommandArgs args) : base(systemController, args) { }
        public override void Execute()
        {
            ScreenRecorderStore.GetRecorder().StartStreaming();
        }
    }


    public class StartStreamingCommandArgs : IRemoteControlCommandArgs
    {
        
    }
}
