using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Elevator.Automation
{
    public class IOContext
    {
        public List<Door> Doors { get; set; } = new List<Door>();
        public Notifier EngineUP { get; set; }
        public Notifier EngineDown { get; set; }

        public IOContext()
        {

        }

        public IOContext(
            ICollection<Door> doors,
            Notifier engineUp,
            Notifier engineDown
            )
        {
            Doors.AddRange(doors);
            EngineUP = engineUp;
            EngineDown = engineDown;
        }

    }
}
