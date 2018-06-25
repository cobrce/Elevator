namespace Elevator.Automation
{
    public class Notifier : AbstractNotifier
    {
        public Notifier() : this(0)
        {

        }
        public Notifier(int plcIoPoint) : base(plcIoPoint)
        {

        }
    }
}
