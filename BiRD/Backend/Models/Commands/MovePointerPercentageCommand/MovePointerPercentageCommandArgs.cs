using BifrostRemoteDesktop.Common.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifrostRemoteDesktop.WPF.Backend.Models.Commands
{
    public class MovePointerPercentageCommandArgs : RemoteControlCommandArgs
    {
        public double PercentageX { get; set; }
        public double PercentageY { get; set; }
    }
}
