using Elevator.Automation;
using Elevator.Automation.IOPoint;
using Elevator.Automation.Notify;
using Elevator.Automation.Types;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace DummyPlcPlugin.Plugin
{
    class DummyPLC : IPLC
    {
        Notifier _engineUpNotifier;
        public Notifier EngineUpNotifier
        {
            get
            {
                return (_engineUpNotifier != null) ? _engineUpNotifier : _engineUpNotifier = new Notifier(new DummyPoint(_upEngine = _io++));
            }
        }

        Notifier _engineDownNotifier;
        public Notifier EngineDownNotifier
        {
            get
            {
                return (_engineDownNotifier != null) ? _engineDownNotifier : _engineDownNotifier = new Notifier(new DummyPoint(_downEngine = _io++));
            }
        }

        enum iopoint
        {
            openDoor,
            closeDoor,
            levelButton,
            doorOpenSensor,
            doorClosedSensor,
            positionSensor,
        };

        int _io = 0;
        int _numberOfDoors;
        int _nPointsPerDoor = 6;
        bool _running = false;
        bool _connected = false;

        private int _msTimeout;
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

        uint _timer = 0;
        public uint Timer { get { return _timer; } }
        DiscreteTimer _openTimer;

        public DummyPLC(int numberOfDoors = 3, int msTimeout = 20, uint msWaitOpen = 1000)
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
                if (_running)
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
                                {
                                    CloseDoor();
                                    state = states.close;
                                }
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
            for (int i = 0; i < _numberOfDoors; i++)
                _doors[i] = Generatedoor(i, _io++, _io++, _io++, _io++, _io++, _io++);
            return _doors;
        }

        private Door Generatedoor(int i, params int[] points)
        {
            _nPointsPerDoor = points.Length;
            return new Door(
                new DummyPoint(points[0]),
                new DummyPoint(points[1]),
                new DummyPoint(points[2]),
                new DummyPoint(points[3]),
                new DummyPoint(points[4]),
                new DummyPoint(points[5])
            );
        }

        public int? Read(IPoint output)
        {
            if (!_connected) return null;

            if (output == EngineUpNotifier.PlcIoPoint)
                return _engine[0];

            else if (output == EngineDownNotifier.PlcIoPoint)
                return _engine[1];

            else
                return ReadIoDoor(output.ByteIndex);

        }

        private int? ReadIoDoor(int output)
        {
            if (GetOutputIndex(output, out int i, out int j))
                return _ioDoors[i][j];
            return null;

        }

        private bool GetOutputIndex(int outputIndex, out int doorindex, out int point)
        {
            doorindex = 0;
            point = 0;

            for (int i = 0; i < Doors.Length; i++)
            {
                Door door = Doors[i];
                doorindex = i;
                if (outputIndex == door.OpenDoor.ByteIndex)
                {
                    point = 0;
                    return true;
                }

                else if (outputIndex == door.CloseDoor.ByteIndex)
                {
                    point = 1;
                    return true;
                }

                else if (outputIndex == door.LevelButton.ByteIndex)
                {
                    point = 2;
                    return true;
                }
                else if (outputIndex == door.DoorOpenSensor.ByteIndex)
                {
                    point = 3;
                    return true;
                }
                else if (outputIndex == door.DoorClosedSensor.ByteIndex)
                {
                    point = 4;
                    return true;
                }
                else if (outputIndex == door.PositionSensor.ByteIndex)
                {
                    point = 5;
                    return true;
                }
            }
            return false;
        }

        public void Write(IPoint input, int state)
        {
            if (_connected)
                if (GetOutputIndex(input.ByteIndex, out int i, out int j))
                    _ioDoors[i][j] = state;
        }

        public bool Connect()
        {
            return _connected = true;
        }

        public void Disconnect()
        {
            _connected = false;
        }

        public void Run()
        {
            _running = true;
        }

        public void Stop()
        {
            _running = false;
        }

        public string Name { get; set; } = "Dummy PLC";

    }
}
