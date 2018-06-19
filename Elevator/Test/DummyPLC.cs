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
        enum iopoint
        {
            levelButton,
            doorOpenSensor,
            doorClosedSensor,
            positionSensor,
        };

        int _io = 0;
        Shape _levelArrow;
        private int _msTimeout;
        Dictionary<int, int> _ioPointDoorIndexDictionary = new Dictionary<int, int>();
        int _upEngine, _downEngine;
        BackgroundWorker backgroundWorker = new BackgroundWorker();

        // current/previous values of doors inputs
        int[][] _ioDoors = new int[3][];
        int[][] _ioDoorsPrev = new int[3][];
        // engine Up and engine down state
        int[] _engine = new int[2];
        // position of elevator (updated from state of sensors)
        int _elevatorPosition = 0;
        // a queue of pressed buttons
        Queue<int> pressedButtons = new Queue<int>();

        // used to count time
        int _timer = 0;

        public DummyPLC(Shape levelArrow, int msTimeout = 20)
        {
            _levelArrow = levelArrow;
            _msTimeout = msTimeout;
            for (int i = 0; i < 3; i++)
            {
                _ioDoors[i] = new int[4];
                _ioDoorsPrev[i] = new int[4];
            }
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                lock (_ioDoors)
                {
                    for (int i = 0; i < _ioDoors.Length; i++)
                        CheckButtonPress(i);

                    ReadElevatorPosition();
                    MoveElevator();
                    CopyState();
                }
                Thread.Sleep(_msTimeout);
            }
        }

        private void ReadElevatorPosition()
        {
            for (int i = 0; i < _ioDoors.Length; i++)
                if (_ioDoors[i][(int)iopoint.positionSensor] == 1)
                    _elevatorPosition = i;
        }

        private void MoveElevator()
        {
            if (pressedButtons.Count > 0)
            {
                // if (_ioDoors[_elevatorPosition][(int)iopoint.doorClosedSensor] == 1)
                {
                    int dest = pressedButtons.Peek();
                    if (dest > _elevatorPosition)
                    {
                        _engine[0] = 1;
                        _engine[1] = 0;
                    }
                    else if (dest < _elevatorPosition)
                    {
                        _engine[0] = 0;
                        _engine[1] = 1;
                    }
                    else
                    {
                        _engine[0] = _engine[1] = 0;
                        pressedButtons.Dequeue();
                        OpenDoor();
                    }
                }
            }
        }

        private void OpenDoor()
        {

        }

        private void CopyState()
        {
            for (int i = 0; i < _ioDoors.Length; i++)
                for (int j = 0; j < 4; j++)
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
        Door[] GenerateDoors()
        {
            _doors = new Door[3];
            _firstdoorIO = _io;
            for (int i = 0; i < 3; i++)
                _doors[i] = Generatedoor(i, _io++, _io++, _io++, _io++);
            return _doors;
        }

        private Door Generatedoor(int i, int levelButton, int doorOpenSensor, int doorClosedSensor, int positionSensor)
        {
            // associate door index to it's IO points for future use
            _ioPointDoorIndexDictionary[levelButton] = i;
            _ioPointDoorIndexDictionary[doorOpenSensor] = i;
            _ioPointDoorIndexDictionary[doorClosedSensor] = i;
            _ioPointDoorIndexDictionary[positionSensor] = i;
            return new Door(levelButton, doorOpenSensor, doorClosedSensor, positionSensor);
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
                new Notifier(_upEngine = _io++, _levelArrow),
                new Notifier(_downEngine = _io++, _levelArrow)
                );
        }

        public int? Read(int output)
        {
            if (output == _upEngine)
                return _engine[0];
            else if (output == _downEngine)
                return _engine[1];
            return null;
        }

        public void Write(int input, int state)
        {
            if (_ioPointDoorIndexDictionary.TryGetValue(input, out int index))
                _ioDoors[index][RelativeIndex(input)] = state;
        }

        private int RelativeIndex(int input)
        {
            return (input - _firstdoorIO) % 4;
        }
    }
}
