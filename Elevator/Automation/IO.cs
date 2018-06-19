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
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerAsync();
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (_running)
            {
                ReadState(_ioContext.EngineUP);
                ReadState(_ioContext.EngineDown);
                Thread.Sleep(_msTimeout);
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
    }
}
