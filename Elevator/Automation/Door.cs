﻿using System.Collections.Generic;
using System.Linq;

namespace Elevator.Automation
{
    public class Door
    {
        public int LevelButton { get; set; }

        private int _openDoor;
        public int OpenDoor
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

        public int DoorOpenSensor { get; set; }

        private int _closeDoor;
        public int CloseDoor
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
        public int DoorClosedSensor { get; set; }
        public int PositionSensor { get; set; }

        public static List<Door> EmptyDoors(int count)
        {
            return new Door[count].ToList();
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
        public Door(int openDoor, int closeDoor, int levelButton, int doorOpenSensor, int doorClosedSensor, int positionSensor)
        {
            LevelButton = levelButton;
            OpenDoor = openDoor;
            DoorOpenSensor = doorOpenSensor;
            CloseDoor = closeDoor;
            PositionSensor = positionSensor;
            DoorClosedSensor = doorClosedSensor;
        }

        public Notifier OpenDoorNotifier { get; set; } = new Notifier(0);
        public Notifier CloseDoorNotifier { get; set; } = new Notifier(0);
    }
}
