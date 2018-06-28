using Elevator.Automation.IOPoint;
using Elevator.Automation.Notify;
using Elevator.Automation.Types;
using System;
using System.ComponentModel;
using System.Threading;

namespace Elevator.Automation.IOReadWrite
{
    // this class is responsible to read/write from/to PLC outputs/inputs
    public class IO
    {
        IPLC PLC { get; }

        public delegate void OpenClosedSensorsDelegate(int level, out bool open, out bool close);
        public delegate bool[] ReadElevatorSensorsDelegate();
        public ReadElevatorSensorsDelegate ReadElevatorSensors { get; set; }
        public OpenClosedSensorsDelegate OpenClosedSensors { get; set; }

        int _msTimeout;
        public IOContext IOContext { get; }
        BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private bool _running = false;

        public IO(IPLC plc, IOContext ioContext, int msTimeout = 20) : this()
        {
            _msTimeout = msTimeout;
            IOContext = ioContext;
            PLC = plc;
        }

        private IO()
        {
            _backgroundWorker.DoWork += PollingLoop;
            _backgroundWorker.RunWorkerAsync();
        }

        private void PollingLoop(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (_running)
                    try
                    {
                        ReadGUI(); // GUI => PLC
                        ReadPLC(); // PLC => GUI
                    }
                    catch { }
                Thread.Sleep(_msTimeout);
            }
        }

        private void ReadPLC()
        {
            ReadState(IOContext.EngineUP);
            ReadState(IOContext.EngineDown);

            foreach (Door door in IOContext.Doors)
            {
                ReadState(door.CloseDoorNotifier);
                ReadState(door.OpenDoorNotifier);
            }
        }

        private void ReadGUI()
        {
            ReadElevatorPosition();
            ReadOpenCloseSensors();
        }

        private void ReadOpenCloseSensors()
        {
            if (OpenClosedSensors != null)
                for (int i = 0; i < IOContext.Doors.Count; i++)
                {
                    OpenClosedSensors(i, out bool open, out bool closed);
                    SetDoorOpenSensor(i, open);
                    SetDoorClosesSensor(i, closed);
                }
        }

        private void ReadElevatorPosition()
        {
            if (ReadElevatorSensors != null)
            {
                bool[] elevatorSensors = ReadElevatorSensors();
                for (int i = 0; i < elevatorSensors.Length; i++)
                    SetDoorPositionSensor(i, elevatorSensors[i]);
            }
        }

        internal void PressButton(int level)
        {
            if (level < IOContext.Doors.Count)
                PulseOnDigitalInput(IOContext.Doors[level].LevelButton);
        }

        private void PulseOnDigitalInput(IPoint input)
        {
            new Thread(() =>
            {
                WriteOnDigitalInput(input, 1);
                Thread.Sleep(100);
                WriteOnDigitalInput(input, 0);
            }).Start();
        }

        private void WriteOnDigitalInput(IPoint input, int state) => PLC.Write(input, state);
        private int? ReadBoolAsInt(IPoint output) => PLC.Read(output);
        private void ReadState(Notifier notifier) => notifier.SetState(this, ReadBoolAsInt(notifier.PlcIoPoint));
        private void SetDoorPositionSensor(int level, bool sensorState) => WriteOnDigitalInput(IOContext.Doors[level].PositionSensor, sensorState ? 1 : 0);
        private void SetDoorOpenSensor(int level, bool sensorState) => WriteOnDigitalInput(IOContext.Doors[level].DoorOpenSensor, sensorState ? 1 : 0);
        private void SetDoorClosesSensor(int level, bool sensorState) => WriteOnDigitalInput(IOContext.Doors[level].DoorClosedSensor, sensorState ? 1 : 0);

        public void Run()
        {
            PLC.Run();
            _running = true;
        }
        public void Stop()
        {
            PLC.Stop();
            _running = false;
        }
        public bool Connect() => PLC.Connect();
        public void Disconnect() => PLC.Disconnect();
        public override string ToString() => PLC.Name;
    }
}
