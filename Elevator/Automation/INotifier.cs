namespace Elevator.Automation
{
    public interface INotifier
    {
        int PlcIoPoint { get; }

        int? GetState();
        void SetState(IO sender, int? value);
    }
}