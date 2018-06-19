using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elevator.Automation
{
    class IOContext
    {
        Door[] _doors = new Door[3];
        public Door[] Doors { get { return _doors; } }

        Notifier _engineUP, _engineDown;
        public Notifier EngineUP { get { return _engineUP; } }
        public Notifier EngineDown { get { return _engineDown; } }

        /// <summary>
        /// this class defines the hardware config of the plc
        /// </summary>
        /// <param name="doors">collection of doors</param>
        /// <param name="engineUpDown">Notifiers containing the PLC output telling wich direction the engline is moving</param>
        public IOContext(
            ICollection<Door> doors,
            Tuple<Notifier, Notifier> engineUpDown
            )
        {
            CopyDoors(doors);
            CopyEngine(engineUpDown);
        }

        private void CopyEngine(Tuple<Notifier, Notifier> engineUpDown)
        {
            _engineUP = engineUpDown.Item1;
            _engineDown = engineUpDown.Item2;
        }

        private void CopyDoors(ICollection<Door> doors)
        {
            _doors = doors.ToArray();
        }
    }
    struct Door
    {
        public int LevelButton;
        public int DoorOpenSensor;
        public int DoorClosedSensor;
        public int PositionSensor;

        /// <summary>
        /// 
        /// <param name="levelButton">The PLC input notified when button is pressed</param>
        /// <param name="doorOpenSensor">The PLC input telling when the door is fully open</param>
        /// <param name="doorClosedSensor">The PLC input telling when the door is fully closed</param>
        /// <param name="positionSensor">The PLC input telling that the elevator arrived to this level</param>
        public Door(int levelButton, int doorOpenSensor, int doorClosedSensor, int positionSensor)
        {
            LevelButton = levelButton;
            DoorOpenSensor = doorOpenSensor;
            PositionSensor = positionSensor;
            DoorClosedSensor = doorClosedSensor;
        }
    }
}
