using Elevator.Automation.IOPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Step7PlcsimPlugin
{
    class S7Point : AbstractPoint
    {
        public S7Point(int byteIndex, int bitIndex, PointType pointType = PointType.pBit) : base(0, byteIndex, bitIndex, pointType)
        {

        }

        public S7Point()
        {
        }

        protected override string GetFormatted() => $"{ByteIndex}.{BitIndex:d1}";
        public override string RegexPattern => @"^(?<byte>[0-9]{1,2})\.(?<bit>[0-7]{1})$";
    }
}
