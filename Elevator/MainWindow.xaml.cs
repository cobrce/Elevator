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
        IAnimation _moveUpDown = new MoveUpDown(1);
        Shape[] _doorsShapes;

        public MainWindow()
        {
            InitializeComponent();
            InitializeAutomation();
            _doorsShapes = new Shape[] { door0, door1, door2 };
        }

        private void InitializeAutomation()
        {
            _plc = new DummyPLC(position);
            /// this is just a dummy plc, ordinary the doors and engine are not defined in IPLC
            ///
            SetEngineEventHandlers(_plc.Engines.Item1, _plc.Engines.Item2);
            _io = new IO(_plc, new IOContext(_plc.Doors, _plc.Engines));
        }

        private void SetEngineEventHandlers(Notifier engineUp, Notifier engineDown)
        {
            engineUp.LevelHigh += EngineUp_LevelHigh;
            engineDown.LevelHigh += EngineDown_LevelHigh;
        }

        private void EngineDown_LevelHigh(object sender, EventArgs e)
        {
            Animate(0);
        }

        private void EngineUp_LevelHigh(object sender, EventArgs e)
        {
            Animate(1);
        }

        private void Animate(int direction)
        {
            position.Dispatcher.Invoke((Action)(() => { AnimateInvoked(direction); }));
        }

        private void AnimateInvoked(int direction)
        {
            _moveUpDown.Animate(position, direction);
            ElevatorMoved();
        }

        private void ElevatorMoved()
        {
            double top = GetMiddlePosition(position);
            for (int i = 0; i < _doorsShapes.Length; i++)
                _io.SetDoorPositionSensor(i, ComparePositionToDoor(top, _doorsShapes[i]));
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
