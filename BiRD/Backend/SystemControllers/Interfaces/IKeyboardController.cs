using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiRD.Backend.SystemControllers.Interfaces
{
    public interface IKeyboardController
    {
        void PressKey(int vKeyCode);
        void ReleaseKey(int vKeyCode);
    }
}
