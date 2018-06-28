using Elevator.Automation.IOReadWrite;

namespace Elevator.Plugins
{
    public interface IPlugin
    {
        IO Register();
    }
}
