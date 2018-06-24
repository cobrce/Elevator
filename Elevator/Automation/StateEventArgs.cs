using System;

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
