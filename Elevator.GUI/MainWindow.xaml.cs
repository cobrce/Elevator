using Elevator.Animation;
using Elevator.Automation;
using Elevator.Automation.IOReadWrite;
using Elevator.Automation.Notify;
using Elevator.Automation.Types;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Elevator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IO _selectedIO;
        IAnimation _moveUpDown;
        AbstractOpenCloseDoorAnimation _doorsOpenClose;

        Shape[] _openDoorsShapes;
        public MainWindow()
        {
            InitializeComponent();
            InitShapes();
            InitAnimations();
            SelectPLC();
        }

        private void InitShapes()
        {
            _openDoorsShapes = new Shape[] { opendoor0, opendoor1, opendoor2 };
        }

        private void SelectPLC()
        {
            PLCSelect select = new PLCSelect();

            if (select.ShowDialog() == true && select.SelectedIO != null)
                SelectIO(select.SelectedIO);
            else
                Close();
        }

        private void InitAnimations()
        {
            
            _moveUpDown = new MoveUpDown(1);
            _doorsOpenClose = new OpenCloseDoor(1, door0.Width - 10, 2);
        }

        private void SelectIO(IO selectedIO)
        {
            _selectedIO = selectedIO;
            _selectedIO.OpenClosedSensors = OpenClosedSensors;
            _selectedIO.ReadElevatorSensors = ReadElevatorSensors;

            SetEngineEventHandlers(
                _selectedIO.IOContext.EngineUP,
                _selectedIO.IOContext.EngineDown,
                _selectedIO.IOContext.Doors.ToArray()
                );

            RunIO();
        }

        private void RunIO()
        {
            if (_selectedIO.Connect())
                _selectedIO.Run();
            else
            {
                MessageBox.Show("Couldn't connect, quitting");
                Close();
            }
        }


        private void SetEngineEventHandlers(Notifier engineUp, Notifier engineDown, Door[] doors)
        {
            if (doors.Length != _openDoorsShapes.Length)
                throw new ArgumentOutOfRangeException($"The count of openCloseDoorNotifiers should be {_openDoorsShapes.Length} (equal to the number of doors)");

            engineUp.LevelHigh += EngineUp_LevelHigh;
            engineDown.LevelHigh += EngineDown_LevelHigh;

            for (int i = 0; i < doors.Length; i++)
            {
                DoorOpenCloseWrapper wrapper = new DoorOpenCloseWrapper(i, DoorOpenClose);
                doors[i].OpenDoorNotifier.LevelHigh += wrapper.Open;
                doors[i].CloseDoorNotifier.LevelHigh += wrapper.Close;
            }
        }

        class DoorOpenCloseWrapper
        {
            private int _level;
            private Action<object, int, int> _doorOpenClosedMethod;

            public DoorOpenCloseWrapper(int level, Action<object, int, int> doorOpenCloseMethod)
            {
                _level = level;
                _doorOpenClosedMethod = doorOpenCloseMethod;
            }
            public void Open(object sender, EventArgs args)
            {
                _doorOpenClosedMethod(sender, _level, 1);
            }
            public void Close(object sender, EventArgs args)
            {
                _doorOpenClosedMethod(sender, _level, 0);
            }
        }

        private void EngineDown_LevelHigh(object sender, EventArgs e)
        {
            if (sender is IO io && io == _selectedIO)
                UpDownAnimate(0);
        }

        private void EngineUp_LevelHigh(object sender, EventArgs e)
        {
            if (sender is IO io && io == _selectedIO)
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
            }));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="open">1 open, 0 close</param>
        private void DoorOpenClose(object sender, int level, int open)
        {
            if (sender is IO io && io == _selectedIO)
                _doorsOpenClose.Animate(_openDoorsShapes[level], open);
        }

        private void OpenClosedSensors(int level, out bool open, out bool closed)
        {
            open = (bool)Dispatcher.Invoke((Func<bool>)((() => { return _doorsOpenClose.isOpen(_openDoorsShapes[level]); })));
            closed = (bool)Dispatcher.Invoke((Func<bool>)(() => { return _doorsOpenClose.isClosed(_openDoorsShapes[level]); }));
        }

        private bool[] ReadElevatorSensors()
        {
            return (bool[])Dispatcher.Invoke((Func<bool[]>)(() =>
          {
              bool[] states = new bool[_openDoorsShapes.Length];
              double top = GetMiddlePosition(position);
              for (int i = 0; i < _openDoorsShapes.Length; i++)
                  states[i] = ComparePositionToDoor(top, _openDoorsShapes[i]);
              return states;
          }));
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
                    _selectedIO.PressButton(level);
                }
            }
        }
    }
}
