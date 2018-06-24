using Elevator.Automation;
using Elevator.Plugins;

namespace Elevator.Test
{
    class DummyPlugin : AbstractPlugin
    {
        public override IO Register()
        {
            DummyPLC dummy = new DummyPLC();
            return Register(dummy, dummy.Doors, dummy.EngineUpNotifier, dummy.EngineDownNotifier);
        }
    }
}
