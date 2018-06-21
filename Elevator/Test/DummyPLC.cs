using Elevator.Automation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Shapes;

namespace Elevator.Test
{
    class DummyPLC : IPLC
    {
        private int _numberOfDoors;

        enum iopoint
        {
            levelButton,
            openDoor,
            doorOpenSensor,
            closeDoor,
            doorClosedSensor,
            positionSensor,
        };

        int _io = 0;
        int _nPointsPerDoor = 6;

        private int _msTimeout;
        Dictionary<int, int> _ioPointDoorIndexDictionary = new Dictionary<int, int>();
        int _upEngine, _downEngine;
        BackgroundWorker backgroundWorker = new BackgroundWorker();

        // current/previous values of doors inputs
        int[][] _ioDoors;
        int[][] _ioDoorsPrev;
        // engine Up and engine down state
        int[] _engine = new int[2];
        // position of elevator (updated from state of sensors)
        int _elevatorPosition = 0;
        // a queue of pressed buttons
        Queue<int> pressedButtons = new Queue<int>();

        // used (not yet) to count time
        uint _timer = 0;
        public uint Timer { get { return _timer; } }
        DiscreteTimer _openTimer;

        public DummyPLC(int numberOfDoors, int msTimeout = 20, uint msWaitOpen = 1000)
        {
            _numberOfDoors = numberOfDoors;
            _msTimeout = msTimeout;
            _openTimer = new DiscreteTimer(this, msWaitOpen);
            _ioDoors = new int[numberOfDoors][];
            _ioDoorsPrev = new int[numberOfDoors][];
            for (int i = 0; i < Doors.Length; i++)
            {
                _ioDoors[i] = new int[_nPointsPerDoor];
                _ioDoorsPrev[i] = new int[_nPointsPerDoor];
            }

            backgroundWorker.DoWork += AutomationLoop;
            backgroundWorker.RunWorkerAsync();
        }

        enum states
        {
            wait,
            move,
            open,
            waitopen,
            close,
        }
        private void AutomationLoop(object sender, DoWorkEventArgs e)
        {
            states state = states.wait;
            while (true)
            {
                lock (_ioDoors)
                {
                    for (int i = 0; i < _ioDoors.Length; i++)
                        CheckButtonPress(i);

                    switch (state)
                    {
                        case states.wait:
                        case states.move:
                            ReadElevatorPosition();
                            state = MoveElevator();
                            break;
                        case states.open:
                            if (isDoorOpen())
                                state = states.waitopen;
                            break;
                        case states.waitopen:
                            if (_openTimer.IsTimeUp())
                                CloseDoor();
                            state = states.close;
                            break;
                        case states.close:
                            if (isDoorClosed())
                                state = states.wait;
                            break;
                    }
                    CopyState();
                }
                Thread.Sleep(_msTimeout);
                _timer += (uint)_msTimeout;
            }
        }

        private bool isDoorOpen()
        {
            if (_ioDoors[_elevatorPosition][(int)iopoint.doorOpenSensor] == 1)
            {
                _openTimer.Init();
                return true;
            }
            return false;
        }

        private bool isDoorClosed()
        {
            return (_ioDoors[_elevatorPosition][(int)iopoint.doorClosedSensor] == 1);
        }

        private void ReadElevatorPosition()
        {
            for (int i = 0; i < _ioDoors.Length; i++)
                if (_ioDoors[i][(int)iopoint.positionSensor] == 1)
                    _elevatorPosition = i;
        }

        private states MoveElevator()
        {
            if (pressedButtons.Count > 0)
            {
                //if (_ioDoors[_elevatorPosition][(int)iopoint.doorClosedSensor] == 1)
                {
                    int dest = pressedButtons.Peek();
                    if (dest > _elevatorPosition) // moving up
                    {
                        _engine[0] = 1;
                        _engine[1] = 0;
                        return states.move;
                    }
                    else if (dest < _elevatorPosition) // moving down
                    {
                        _engine[0] = 0;
                        _engine[1] = 1;
                        return states.move;
                    }
                    else // arrived, open
                    {
                        _engine[0] = _engine[1] = 0;
                        pressedButtons.Dequeue();
                        OpenDoor();
                        return states.open;
                    }
                }
            }
            return states.wait;
        }

        private void OpenDoor()
        {
            _ioDoors[_elevatorPosition][(int)iopoint.closeDoor] = 0;
            _ioDoors[_elevatorPosition][(int)iopoint.openDoor] = 1;
        }
        private void CloseDoor()
        {
            _ioDoors[_elevatorPosition][(int)iopoint.openDoor] = 0;
            _ioDoors[_elevatorPosition][(int)iopoint.closeDoor] = 1;
        }

        private void CopyState()
        {
            for (int i = 0; i < _ioDoors.Length; i++)
                for (int j = 0; j < _ioDoors[i].Length; j++)
                    _ioDoorsPrev[i][j] = _ioDoors[i][j];
        }

        private void CheckButtonPress(int index)
        {
            if (
                _ioDoorsPrev[index][(int)iopoint.levelButton] == 0 &&
                _ioDoors[index][(int)iopoint.levelButton] == 1 &&
                (pressedButtons.Count == 0 || pressedButtons.Peek() != index)
                )
                pressedButtons.Enqueue(index);
        }

        Door[] _doors;
        private int _firstdoorIO;

        public Door[] Doors
        {
            get
            {
                return (_doors != null) ? _doors : _doors = GenerateDoors();
            }
        }

        private Door[] GenerateDoors()
        {
            _doors = new Door[_numberOfDoors];
            _firstdoorIO = _io;
            for (int i = 0; i < _numberOfDoors; i++)
                _doors[i] = Generatedoor(i, _io++, _io++, _io++, _io++, _io++, _io++);
            return _doors;
        }

        private Door Generatedoor(int i, params int[] points)
        {
            _nPointsPerDoor = points.Length;

            // associate door index to it's IO points for future use
            for (int j = 0; j < points.Length; j++)
                _ioPointDoorIndexDictionary[points[j]] = i;

            return new Door(points[0], points[1], points[2], points[3], points[4], points[5]);
        }

        Tuple<Notifier, Notifier> _engines;
        public Tuple<Notifier, Notifier> Engines
        {
            get
            {
                return (_engines != null) ? _engines : _engines = GenerateEngines();
            }
        }

        Tuple<Notifier, Notifier> GenerateEngines()
        {
            return new Tuple<Notifier, Notifier>(
                new Notifier(_upEngine = _io++),
                new Notifier(_downEngine = _io++)
                );
        }

        Tuple<Notifier, Notifier>[] _openCloseDoor;
        public Tuple<Notifier, Notifier>[] OpenCloseDoor
        {
            get
            {
                return (_openCloseDoor != null) ? _openCloseDoor : _openCloseDoor = GenerateOpenCloseDoor();
            }
        }

        private Tuple<Notifier, Notifier>[] GenerateOpenCloseDoor()
        {
            List<Tuple<Notifier, Notifier>> notifiers = new List<Tuple<Notifier, Notifier>>();

            for (int i = 0; i < _numberOfDoors; i++)
                notifiers.Add(
                    new Tuple<Notifier, Notifier>(
                        new Notifier(Doors[i].OpenDoor),
                        new Notifier(Doors[i].CloseDoor)
                    )
                );

            return notifiers.ToArray();
        }

        public int? Read(int output)
        {

            if (output == _upEngine)
                return _engine[0];
            else if (output == _downEngine)
                return _engine[1];
            else
                return ReadIoDoor(output);

        }

        private int? ReadIoDoor(int output)
        {
            if (_ioPointDoorIndexDictionary.TryGetValue(output, out int index))
                return _ioDoors[index][RelativeIndex(output)];
            return null;
        }

        public void Write(int input, int state)
        {
            if (_ioPointDoorIndexDictionary.TryGetValue(input, out int index))
                _ioDoors[index][RelativeIndex(input)] = state;
        }

        private int RelativeIndex(int input)
        {
            return (input - _firstdoorIO) % _nPointsPerDoor;
        }
    }
}
