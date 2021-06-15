using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiRD.Backend.SystemControllers.Interfaces
{
    public interface IMouseController
    {
        public void SetCursorPosition(int x, int y);
        public void SetCursorPosition(double x, double y);
        public void SetCursorPositionPercentage(double percentageX, double percentageY);
        
        public void PressLeftButton();
        public void ReleaseLeftButton();

        public void PressRightButton();
        public void ReleaseRightButton();

        public void PressMiddleButton();
        public void ReleaseMiddleButton();
    }
}
