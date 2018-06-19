using System;
using System.Windows.Shapes;

namespace Elevator.Automation
{
    abstract class AbstractNotifier : INotifier
    {
        protected Shape _shape;
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
                    LevelHigh?.Invoke(_shape, null);
                else
                    LevelLow?.Invoke(_shape, null);
                if (value != State)
                    OnEdge?.Invoke(_shape, new StateEventArgs(value));

                _state = value;
            }
        }

        public AbstractNotifier(int plcIoPoint, Shape shape)
        {
            _plcIoPoint = plcIoPoint;
            _shape = shape;
        }
    }
}
