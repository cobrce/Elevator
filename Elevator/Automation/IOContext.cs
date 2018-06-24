using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elevator.Automation
{
    public class IOContext
    {
        public List<Door> Doors { get; set; } = new List<Door>();

        Notifier _engineUP, _engineDown;
        public Notifier EngineUP { get { return _engineUP; } }
        public Notifier EngineDown { get { return _engineDown; } }
        
        public IOContext(
            ICollection<Door> doors,
            Notifier engineUp,
            Notifier engineDown
            )
        {
            Doors.AddRange(doors);
            _engineUP = engineUp;
            _engineDown = engineDown;
        }

        private void CopyEngine(Tuple<Notifier, Notifier> engineUpDown)
        {
            _engineUP = engineUpDown.Item1;
            _engineDown = engineUpDown.Item2;
        }
    }

}
