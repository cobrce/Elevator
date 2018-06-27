namespace Elevator.Automation
{
    public interface INotifier
    {
        IPoint PlcIoPoint { get; }

        int? GetState();
        void SetState(IO sender, int? value);
    }
}