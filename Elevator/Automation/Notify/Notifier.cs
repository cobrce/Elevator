using Elevator.Automation.IOPoint;

namespace Elevator.Automation.Notify
{
    public class Notifier : AbstractNotifier
    {
        public Notifier()
        {

        }
        public Notifier(IPoint plcIoPoint) : base(plcIoPoint)
        {

        }
    }
}
