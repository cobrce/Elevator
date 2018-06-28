using Elevator.Automation;
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
                    // since these values are to be filled by the user we can just init with default value
                    Door.GenerateDoors(3, new S7Point(0, 0)),
                    new Notifier(new S7Point(2, 0)),
                    new Notifier(new S7Point(2, 1))
                )
            );
        }
    }
}
