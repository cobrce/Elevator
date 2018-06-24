//#define safe
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Elevator.Automation
{
    // this class is responsible to read/write from/to PLC outputs/inputs
    public class IO
    {
        int _msTimeout;
        public IOContext IOContext { get; }

        IPLC PLC { get; }

        public IO(IPLC plc, IOContext ioContext, int msTimeout = 20) : this()
        {
            _msTimeout = msTimeout;
            IOContext = ioContext;
            PLC = plc;
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
                ReadState(IOContext.EngineUP);
                ReadState(IOContext.EngineDown);

                foreach (Door door in IOContext.Doors)
                {
                    ReadState(door.CloseDoorNotifier);
                    ReadState(door.OpenDoorNotifier);
                }

                Thread.Sleep(_msTimeout);
            }
        }
        
        internal void PressButton(int level)
        {
            if (level < IOContext.Doors.Count)
                PulseOnDigitalInput(IOContext.Doors[level].LevelButton);
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

        private void WriteOnDigitalInput(int input, int state) => PLC.Write(input, state);
        private int? ReadBoolAsInt(int output) => PLC.Read(output);
        private void ReadState(Notifier notifier) => notifier.SetState(this, ReadBoolAsInt(notifier.PlcIoPoint));
        public void SetDoorPositionSensor(int level, bool sensorState) => WriteOnDigitalInput(IOContext.Doors[level].PositionSensor, sensorState ? 1 : 0);
        public void SetDoorOpenSensor(int level, bool sensorState) => WriteOnDigitalInput(IOContext.Doors[level].DoorOpenSensor, sensorState ? 1 : 0);
        public void SetDoorClosesSensor(int level, bool sensorState) => WriteOnDigitalInput(IOContext.Doors[level].DoorClosedSensor, sensorState ? 1 : 0);
        public bool Connect() => PLC.Connect();
        public void Disconnect() => PLC.Disconnect();
        public void Run() => PLC.Run();
        public void Stop() => PLC.Stop();
        public override string ToString() => PLC.Name;
    }
}
