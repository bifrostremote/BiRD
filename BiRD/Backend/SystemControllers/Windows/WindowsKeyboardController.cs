using BiRD.Backend.SystemControllers.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace BifrostRemoteDesktop.Common.SystemControllers
{
    public class WindowsKeyboardController : IKeyboardController
    {
        [DllImport("user32.dll")]
        static extern short VkKeyScan(char ch);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        public void PressKey(int vKeyCode)
        {
            const int VKEY_IS_PRESSED = 0x0;
            keybd_event((byte)vKeyCode, 0, VKEY_IS_PRESSED, 0);
        }

        public void ReleaseKey(int vKeyCode)
        {
            const int VKEY_IS_RELEASED = 0x2;
            keybd_event((byte)vKeyCode, 0, VKEY_IS_RELEASED, 0);
        }
    }
}