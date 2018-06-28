using Elevator.Automation.IOPoint;
using Elevator.Automation.Notify;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Elevator.Automation.Types
{
    public class Door
    {
        public IPoint LevelButton { get; set; }
        
        private IPoint _openDoor;
        public IPoint OpenDoor
        {
            get
            {
                return _openDoor;
            }
            set
            {
                _openDoor = value;
                OpenDoorNotifier.PlcIoPoint = value;
            }
        }
        
        public IPoint DoorOpenSensor { get; set; }

        private IPoint _closeDoor;
        public IPoint CloseDoor
        {
            get
            {
                return _closeDoor;
            }
            set
            {
                _closeDoor = value;
                CloseDoorNotifier.PlcIoPoint = value;
            }
        }
        
        public IPoint DoorClosedSensor { get; set; }

        public IPoint PositionSensor { get; set; }

        public static List<Door> GenerateDoors(int count,IPoint defaultPoint)
        {
            List<Door> doors = new List<Door>();
            for (int i = 0; i < count; i++)
                doors.Add(new Door(
                        defaultPoint.Clone(),
                        defaultPoint.Clone(),
                        defaultPoint.Clone(),
                        defaultPoint.Clone(),
                        defaultPoint.Clone(),
                        defaultPoint.Clone()
                    )
                );
            return doors;
        }

        public Door()
        {

        }
        /// <summary>
        /// 
        /// <param name="levelButton">The PLC input notified when button is pressed</param>
        /// <param name="openDoor">The PLC output to open the door</param>
        /// <param name="doorOpenSensor">The PLC input telling when the door is fully open</param>
        /// <param name="closeDoor">The PLC output to close the door</param>
        /// <param name="doorClosedSensor">The PLC input telling when the door is fully closed</param>
        /// <param name="positionSensor">The PLC input telling that the elevator arrived to this level</param>
        public Door(IPoint openDoor, IPoint closeDoor, IPoint levelButton, IPoint doorOpenSensor, IPoint doorClosedSensor, IPoint positionSensor)
        {
            LevelButton = levelButton;
            OpenDoor = openDoor;
            DoorOpenSensor = doorOpenSensor;
            CloseDoor = closeDoor;
            PositionSensor = positionSensor;
            DoorClosedSensor = doorClosedSensor;
        }

        public Notifier OpenDoorNotifier { get; set; } = new Notifier();
        public Notifier CloseDoorNotifier { get; set; } = new Notifier();
    }
}
