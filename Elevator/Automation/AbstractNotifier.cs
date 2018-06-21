using System;
using System.Windows.Shapes;

namespace Elevator.Automation
{
    abstract class AbstractNotifier : INotifier
    {
        protected int _plcIoPoint;
        public event EventHandler LevelHigh;
        public event EventHandler LevelLow;
        public event EventHandler OnEdge;

        virtual public int PlcIoPoint { get { return _plcIoPoint; } }

        protected int? _state;
        public int? State
        {
            get
            {
                return _state;
            }
            set
            {
                if (value > 0)
                    LevelHigh?.Invoke(this, null);
                else
                    LevelLow?.Invoke(this, null);
                if (value != State)
                    OnEdge?.Invoke(this, new StateEventArgs(value));

                _state = value;
            }
        }

        public AbstractNotifier(int plcIoPoint)
        {
            _plcIoPoint = plcIoPoint;
        }
    }
}
