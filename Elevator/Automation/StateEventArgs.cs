using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elevator.Automation
{
    class StateEventArgs : EventArgs
    {
        public StateEventArgs(int? value)
        {
            Value = value;
        }
        int? Value { get; set; }
    }
}
