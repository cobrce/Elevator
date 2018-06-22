#define safe
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Elevator.Automation
{
    // this class is responsible to read/write from/to PLC outputs/inputs
    class IO
    {
        int _msTimeout;
        IOContext _ioContext { get; set; }
        IPLC _plc;

        public IO(IPLC plc, IOContext ioContext, int msTimeout = 20) : this()
        {
            _msTimeout = msTimeout;
            _ioContext = ioContext;
            _plc = plc;
        }

        BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private bool _running = true;

        private IO()
        {
            _backgroundWorker.DoWork += PollingLoop;
            _backgroundWorker.RunWorkerAsync();
        }

        private void PollingLoop(object sender, DoWorkEventArgs e)
        {
            while (_running)
            {
                ReadState(_ioContext.EngineUP);
                ReadState(_ioContext.EngineDown);

                foreach (var notifier in EnumerateNotifiers(_ioContext.OpenCloseDoor))
                    ReadState(notifier);

                Thread.Sleep(_msTimeout);
            }
        }

        private IEnumerable<Notifier> EnumerateNotifiers(Tuple<Notifier, Notifier>[] notifiersTupleArray)
        {
            foreach (var tpl in notifiersTupleArray)
            {
                yield return tpl.Item1;
                yield return tpl.Item2;
            }
        }

        internal void PressButton(int level)
        {
            if (level < _ioContext.Doors.Length)
                PulseOnDigitalInput(_ioContext.Doors[level].LevelButton);
        }

        private void PulseOnDigitalInput(int input)
        {
#if !safe
            new Thread(() =>
            {
#endif
            WriteOnDigitalInput(input, 1);
            Thread.Sleep(100);
            WriteOnDigitalInput(input, 0);
#if !safe
            }).Start();
#endif
        }

        private void WriteOnDigitalInput(int input, int state)
        {
            _plc.Write(input, state);
        }

        private int? ReadBoolAsInt(int output)
        {
            return _plc.Read(output);
        }

        private void ReadState(Notifier notifier)
        {
            notifier.State = ReadBoolAsInt(notifier.PlcIoPoint);
        }

        internal void SetDoorPositionSensor(int level, bool sensorState)
        {
            WriteOnDigitalInput(_ioContext.Doors[level].PositionSensor, sensorState ? 1 : 0);
        }
        internal void SetDoorOpenSensor(int level, bool sensorState)
        {
            WriteOnDigitalInput(_ioContext.Doors[level].DoorOpenSensor, sensorState ? 1 : 0);
        }
        internal void SetDoorClosesSensor(int level, bool sensorState)
        {
            WriteOnDigitalInput(_ioContext.Doors[level].DoorClosedSensor, sensorState ? 1 : 0);
        }
    }
}
