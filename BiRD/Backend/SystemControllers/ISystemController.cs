namespace BifrostRemoteDesktop.Common.SystemControllers
{
    public interface ISystemController
    {
         public WindowsMouseController Mouse { get; }
         public WindowsKeyboardController Keyboard { get; }
    }
}
