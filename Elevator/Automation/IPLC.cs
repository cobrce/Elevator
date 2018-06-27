namespace Elevator.Automation
{
    public interface IPLC
    {
        int? Read(IPoint output);
        void Write(IPoint input, int state);
        bool Connect();
        void Disconnect();
        void Run();
        void Stop();
        string Name { get; set; }
    }
}
