using Elevator.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elevator.Automation.IOPoint
{
    public class Point : AbstractPoint
    {
        public Point(int byteIndex) : base(byteIndex)
        {
        }

        public Point(int deviceIndex, int byteIndex, int bitIndex) : base(deviceIndex, byteIndex, bitIndex)
        {
        }
    }
}
