using BiRD.Backend.SystemControllers.Interfaces;
using System;

namespace BifrostRemoteDesktop.Common.SystemControllers
{
    public class WindowsSystemController : ISystemController
    {


        public IMouseController Mouse { get; }

        public IKeyboardController Keyboard { get; }

        public WindowsSystemController()
        {
            Mouse = new WindowsMouseController();
            Keyboard = new WindowsKeyboardController();
        }

    }
}
