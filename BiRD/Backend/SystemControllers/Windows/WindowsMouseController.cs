using BifrostRemoteDesktop.Common.Models;
using BiRD.Backend.SystemControllers.Interfaces;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace BifrostRemoteDesktop.Common.SystemControllers
{
    public class WindowsMouseController: IMouseController
    {
        // Source: https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-mouseinput
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const int MOUSEEVENTF_RIGHTUP = 0x0010;

        // Source: https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-input
        const int INPUT_MOUSE = 0;

        //Source: https://stackoverflow.com/a/10355905
        #region 
        private struct INPUT
        {
            public UInt32 Type;
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
        }

        private struct MOUSEINPUT
        {
            public Int32 X;
            public Int32 Y;
            public UInt32 MouseData;
            public UInt32 Flags;
            public UInt32 Time;
            public IntPtr ExtraInfo;
        }
        #endregion

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out LPPoint lpPoint);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref LPPoint lpPoint);

        public LPPoint GetMousePosition()
        {
            if (GetCursorPos(out LPPoint point)) return point;
            throw new Exception();
        }

        public void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public void SetCursorPosition(double x, double y)
        {
            SetCursorPos(Convert.ToInt32(x), Convert.ToInt32(y));
        }

        public void SetCursorPositionPercentage(double percentageX, double percentageY)
        {
            //double x = (SystemParameters.PrimaryScreenWidth / 96 * 120) * percentageX;
            //double y = (SystemParameters.PrimaryScreenHeight / 96 * 120) * percentageY;
            double x = (SystemParameters.PrimaryScreenWidth * percentageX);
            double y = (SystemParameters.PrimaryScreenHeight * percentageY);

            SetCursorPosition(x, y);
        }

        public void PressLeftButton()
        {
            InvokeMouseEvent(MOUSEEVENTF_LEFTDOWN);
        }

        public void ReleaseLeftButton()
        {
            InvokeMouseEvent(MOUSEEVENTF_LEFTUP);
        }

        public void PressRightButton()
        {
            InvokeMouseEvent(MOUSEEVENTF_RIGHTDOWN);
        }

        public void ReleaseRightButton()
        {
            InvokeMouseEvent(MOUSEEVENTF_RIGHTUP);
        }

        private static void InvokeMouseEvent(uint flags)
        {
            INPUT input = new INPUT();
            input.Type = INPUT_MOUSE;
            input.Data.Mouse.Flags = flags;

            INPUT[] inputs = new INPUT[] { input };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public void PressMiddleButton()
        {
            throw new NotImplementedException();
        }

        public void ReleaseMiddleButton()
        {
            throw new NotImplementedException();
        }
    }
}
