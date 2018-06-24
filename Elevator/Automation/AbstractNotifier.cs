using System;

namespace Elevator.Automation
{
    public abstract class AbstractNotifier : INotifier
    {
        public event EventHandler LevelHigh;
        public event EventHandler LevelLow;
        public event EventHandler OnEdge;

        virtual public int PlcIoPoint { get; set; }

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

        public AbstractNotifier(int plcIoPoint)
        {
            PlcIoPoint = plcIoPoint;
        }
    }
}
