using System;
using System.Runtime.InteropServices;

namespace BifrostRemoteDesktop.Common.SystemControllers
{
    public class WindowsKeyboardController
    {

        [DllImport("user32.dll")] 
        static extern short VkKeyScan(char ch);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        public void PressKey(char key)
        {
            //keybd_event(VkKeyScan(key));
        }
    }
}