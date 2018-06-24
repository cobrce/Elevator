namespace Elevator.Automation
{
    public interface IPLC
    {
        int? Read(int output);
        void Write(int input, int state);
        bool Connect();
        void Disconnect();
        void Run();
        void Stop();
        string Name { get; set; }
        Notifier EngineUpNotifier { get; }
        Notifier EngineDownNotifier { get; }
    }
}
