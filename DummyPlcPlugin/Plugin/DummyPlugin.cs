using Elevator.Automation;
using Elevator.Plugins;

namespace Elevator.Test
{
    class DummyPlugin : AbstractPlugin
    {
        public override IO Register()
        {
            DummyPLC dummy = new DummyPLC();
            return Register(
                dummy,
                /// the following parameters are not supposed to be declared in a IPLC
                /// this plugin is using this because it's acting as a IPLC and a remote 
                /// simulator at the same time, in real case scenario the IO points 
                /// (or the hardware config) are defined in the remote PLC/simulator
                /// and redefined to the plugin by the using from the GUI.
                dummy.Doors,
                dummy.EngineUpNotifier,
                dummy.EngineDownNotifier
                );
        }
    }
}
