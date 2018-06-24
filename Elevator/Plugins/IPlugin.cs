using Elevator.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elevator.Plugins
{
    public interface IPlugin
    {
        IO Register();
    }
}
