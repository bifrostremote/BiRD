namespace BifrostRemoteDesktop.Common.Models.Commands
{
    public interface IRemoteControlCommand<AssociatedArgumentType> : ICommand where AssociatedArgumentType : IRemoteControlCommandArgs
    {
    }
}
