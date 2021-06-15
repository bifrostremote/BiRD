using BiRD.Backend.SystemControllers.Interfaces;

namespace BifrostRemoteDesktop.Common.SystemControllers
{
    public interface ISystemController
    {
         public IMouseController Mouse { get; }
         public IKeyboardController Keyboard { get; }
    }
}
