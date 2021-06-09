using System;

namespace BifrostRemoteDesktop.Common.SystemControllers
{
    public class WindowsSystemController : ISystemController
    {
        public WindowsMouseController Mouse { get; }
        public WindowsKeyboardController Keyboard { get; }

        public WindowsSystemController()
        {
            Mouse = new WindowsMouseController();
            Keyboard = new WindowsKeyboardController();
        }

    }
}
