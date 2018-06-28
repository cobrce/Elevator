using Elevator.Automation;
using Elevator.Automation.IOReadWrite;
using Elevator.Automation.Notify;
using Elevator.Automation.Types;
using System.Collections.Generic;

namespace Elevator.Plugins
{
    public abstract class AbstractPlugin : IPlugin
    {
        public AbstractPlugin()
        {

        }

        public abstract IO Register();

        protected IO Register(IPLC plc, ICollection<Door> doors, Notifier engineUpNotifier, Notifier engineDownNotifier)
        {
            IO io = new IO(
                            plc,
                            new IOContext(
                                doors,
                                engineUpNotifier,
                                engineDownNotifier
                                )
                            );
            return io;
        }
    }
}
