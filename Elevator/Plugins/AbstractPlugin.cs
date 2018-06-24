using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elevator.Automation;

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
