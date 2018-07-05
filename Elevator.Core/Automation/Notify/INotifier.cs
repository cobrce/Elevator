using Elevator.Automation.IOPoint;
using Elevator.Automation.IOReadWrite;

namespace Elevator.Automation.Notify
{
    public interface INotifier
    {
        IPoint PlcIoPoint { get; }

        int? GetState();
        void SetState(IO sender, int? value);
    }
}