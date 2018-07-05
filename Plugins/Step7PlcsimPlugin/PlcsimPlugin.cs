using Elevator.Automation;
using Elevator.Automation.IOPoint;
using Elevator.Automation.IOReadWrite;
using Elevator.Automation.Notify;
using Elevator.Automation.Types;
using Elevator.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Step7PlcsimPlugin
{
    public class PlcsimPlugin : AbstractPlugin
    {
        public override IO Register()
        {
            return new IO(
                new Plcsim(),
                new IOContext(
                    GenerateDoors(3),
                    new Notifier(new S7Point(4, 6)),
                    new Notifier(new S7Point(4, 7))
                )
            );
        }

        private ICollection<Door> GenerateDoors(int count)
        {
            List<Door> doors = new List<Door>();
            for (int i = 0; i < count; i++)
                doors.Add(
                    new Door(
                        new S7Point(4, i * 2),
                        new S7Point(4, i * 2 + 1),
                        genS7InputPoint(i * 4),
                        genS7InputPoint(i * 4 + 1),
                        genS7InputPoint(i * 4 + 2),
                        genS7InputPoint(i * 4 + 3)
                    )
                );
            return doors;
        }
        private S7Point genS7InputPoint(int i)
        {
            return new S7Point(i / 8, i % 8, segment: MemorySegment.input);
        }
    }
}
