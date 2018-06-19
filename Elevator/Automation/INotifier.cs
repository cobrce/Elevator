using System;

namespace Elevator.Automation
{
    interface INotifier
    {
        int PlcIoPoint { get; }
        int? State { get; set; }
    }
}