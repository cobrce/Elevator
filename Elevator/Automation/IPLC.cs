using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elevator.Automation
{
    interface IPLC
    {
        int? Read(int output);
        void Write(int input, int state);
    }
}
