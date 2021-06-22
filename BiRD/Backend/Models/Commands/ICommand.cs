namespace BifrostRemoteDesktop.Common.Models.Commands
{
    public interface ICommand
    {
        void Execute();
        void Execute(object obj);
    }
}