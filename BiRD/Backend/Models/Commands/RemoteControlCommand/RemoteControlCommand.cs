using BifrostRemoteDesktop.Common.Network;
using BifrostRemoteDesktop.Common.SystemControllers;

namespace BifrostRemoteDesktop.Common.Models.Commands
{
    public abstract class RemoteControlCommand<AssociatedArgumentType> : IRemoteControlCommand<AssociatedArgumentType> where AssociatedArgumentType : IRemoteControlCommandArgs
    {
        protected ISystemController SystemController { get; }
        protected AssociatedArgumentType Args { get; }

        public RemoteControlCommand(ISystemController systemController, AssociatedArgumentType args)
        {
            SystemController = systemController;
            Args = args;
        }

        public abstract void Execute();
        public virtual void Execute(object obj) { }
    }
}
