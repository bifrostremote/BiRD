using BifrostRemoteDesktop.Common.SystemControllers;

namespace BifrostRemoteDesktop.Common.Models.Commands
{
    public abstract class RemoteControlCommand<AssociatedArgumentType> : ICommand where AssociatedArgumentType : RemoteControlCommandArgs
    {
        protected ISystemController SystemController { get; }
        protected AssociatedArgumentType Args { get; }

        public RemoteControlCommand(ISystemController systemController, AssociatedArgumentType args)
        {
            SystemController = systemController;
            Args = args;
        }

        public abstract void Execute();
    }
}
