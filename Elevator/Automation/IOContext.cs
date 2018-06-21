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
        Tuple<Notifier, Notifier>[] _openCloseDoor;
        public Notifier EngineUP { get { return _engineUP; } }
        public Notifier EngineDown { get { return _engineDown; } }
        public Tuple<Notifier, Notifier>[] OpenCloseDoor { get { return _openCloseDoor; } }

        public IOContext(
            ICollection<Door> doors,
            Notifier engineUp,
            Notifier engineDown,
            Tuple<Notifier, Notifier>[] openCloseDoor
            )
        {
            CopyDoors(doors);
            _openCloseDoor = openCloseDoor;
            _engineUP = engineUp;
            _engineDown = engineDown;
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

}
