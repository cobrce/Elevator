using Elevator.Animation;
using Elevator.Automation;
using Elevator.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Elevator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DummyPLC _plc;
        private IO _io;
        IAnimation _moveUpDown;
        AbstractOpenCloseDoorAnimation _doorsOpenClose;

        Shape[] _openDoorsShapes;
        public MainWindow()
        {
            InitializeComponent();
            _openDoorsShapes = new Shape[] { opendoor0, opendoor1, opendoor2 };


            InitializeAutomation();
            InitAnimations();

        }

        private void InitAnimations()
        {
            _moveUpDown = new MoveUpDown(1);
            _doorsOpenClose = new OpenCloseDoor(1, door0.Width - 10, 2);
        }

        private void InitializeAutomation()
        {
            /// this is just a dummy plc, ordinary the doors and engine are not defined in IPLC
            _plc = new DummyPLC(3);

            SetEngineEventHandlers(_plc.Engines.Item1, _plc.Engines.Item2, _plc.OpenCloseDoor);

            _io = new IO(
                _plc,
                new IOContext(
                    _plc.Doors,
                    _plc.Engines.Item1,
                    _plc.Engines.Item2,
                    _plc.OpenCloseDoor
                    )
                );
        }

        private void SetEngineEventHandlers(Notifier engineUp, Notifier engineDown, Tuple<Notifier, Notifier>[] openCloseDoorNotifiers)
        {
            if (openCloseDoorNotifiers.Length != _openDoorsShapes.Length)
                throw new ArgumentOutOfRangeException($"The count of openCloseDoorNotifiers should be {_openDoorsShapes.Length} (equal to the number of doors)");

            engineUp.LevelHigh += EngineUp_LevelHigh;
            engineDown.LevelHigh += EngineDown_LevelHigh;

            for (int i = 0; i < openCloseDoorNotifiers.Length; i++)
            {
                DoorOpenCloseWrapper wrapper = new DoorOpenCloseWrapper(i, DoorOpenClose);
                openCloseDoorNotifiers[i].Item1.LevelHigh += wrapper.Open;
                openCloseDoorNotifiers[i].Item2.LevelHigh += wrapper.Close;
            }
        }

        class DoorOpenCloseWrapper
        {
            private int _level;
            private Action<int, int> _doorOpenClosedMethod;

            public DoorOpenCloseWrapper(int level, Action<int, int> doorOpenCloseMethod)
            {
                _level = level;
                _doorOpenClosedMethod = doorOpenCloseMethod;
            }
            public void Open(object sender, EventArgs args)
            {
                _doorOpenClosedMethod(_level, 1);

            }
            public void Close(object sender, EventArgs args)
            {
                _doorOpenClosedMethod(_level, 0);
            }
        }

        private void EngineDown_LevelHigh(object sender, EventArgs e)
        {
            UpDownAnimate(0);
        }

        private void EngineUp_LevelHigh(object sender, EventArgs e)
        {
            UpDownAnimate(1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction">1 up, 0 down</param>
        private void UpDownAnimate(int direction)
        {
            position.Dispatcher.Invoke((Action)(() =>
            {
                _moveUpDown.Animate(position, direction);
                ElevatorMoved();
            }));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="open">1 open, 0 close</param>
        private void DoorOpenClose(int level, int open)
        {
            _doorsOpenClose.Animate(_openDoorsShapes[level], open);
            _io.SetDoorOpenSensor(level, _doorsOpenClose.isOpen(_openDoorsShapes[level]));
            _io.SetDoorClosesSensor(level, _doorsOpenClose.isClosed(_openDoorsShapes[level]));
        }

        private void ElevatorMoved()
        {
            double top = GetMiddlePosition(position);
            for (int i = 0; i < _openDoorsShapes.Length; i++)
                _io.SetDoorPositionSensor(i, ComparePositionToDoor(top, _openDoorsShapes[i]));
        }

        private bool ComparePositionToDoor(double top, Shape door, double tolerance = 2)
        {
            double doortop = GetMiddlePosition(door);
            return (top >= (doortop - tolerance) && top <= (doortop + tolerance));
        }

        private double GetMiddlePosition(Shape shape)
        {
            double top = Canvas.GetTop(shape);
            if (double.IsNaN(top))
                top = 0;
            top += shape.Height / 2;
            return top;
        }
        private void btnsClick(object sender, RoutedEventArgs e)
        {
            if (sender is Control control && control.Tag != null)
            {
                if (int.TryParse(control.Tag.ToString(), out int level))
                {
                    _io.PressButton(level);
                }
            }
        }
    }
}
