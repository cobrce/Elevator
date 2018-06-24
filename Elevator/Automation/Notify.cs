using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace Elevator.Automation
{
    public class Notifier : AbstractNotifier
    {
        public Notifier(int plcIoPoint) : base(plcIoPoint)
        {

        }
    }
}
