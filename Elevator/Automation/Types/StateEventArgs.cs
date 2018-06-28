using System;

namespace Elevator.Automation.Types
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
