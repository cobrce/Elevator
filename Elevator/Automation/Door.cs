using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elevator.Automation
{
    struct Door
    {
        public int LevelButton;
        public int OpenDoor;
        public int DoorOpenSensor;
        public int CloseDoor;
        public int DoorClosedSensor;
        public int PositionSensor;

        /// <summary>
        /// 
        /// <param name="levelButton">The PLC input notified when button is pressed</param>
        /// <param name="openDoor">The PLC output to open the door</param>
        /// <param name="doorOpenSensor">The PLC input telling when the door is fully open</param>
        /// <param name="closeDoor">The PLC output to close the door</param>
        /// <param name="doorClosedSensor">The PLC input telling when the door is fully closed</param>
        /// <param name="positionSensor">The PLC input telling that the elevator arrived to this level</param>
        public Door(int levelButton, int openDoor, int doorOpenSensor, int closeDoor, int doorClosedSensor, int positionSensor)
        {
            LevelButton = levelButton;
            OpenDoor = openDoor;
            DoorOpenSensor = doorOpenSensor;
            CloseDoor = closeDoor;
            PositionSensor = positionSensor;
            DoorClosedSensor = doorClosedSensor;
        }
    }
}
