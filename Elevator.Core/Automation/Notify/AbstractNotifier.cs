﻿using Elevator.Automation.IOPoint;
using Elevator.Automation.IOReadWrite;
using Elevator.Automation.Types;
using System;

namespace Elevator.Automation.Notify
{
    [Serializable]
    public abstract class AbstractNotifier : INotifier
    {
        public event EventHandler LevelHigh;
        public event EventHandler LevelLow;
        public event EventHandler OnEdge;

        virtual public IPoint PlcIoPoint { get; set; }

        protected int? _state;

        public int? GetState()
        {
            return _state;
        }
        public void SetState(IO sender, int? value)
        {
            if (value > 0)
                LevelHigh?.Invoke(sender, null);
            else
                LevelLow?.Invoke(sender, null);
            if (value != GetState())
                OnEdge?.Invoke(sender, new StateEventArgs(value));

            _state = value;
        }

        public AbstractNotifier()
        {

        }

        public AbstractNotifier(IPoint plcIoPoint)
        {
            PlcIoPoint = plcIoPoint;
        }

    }
}
