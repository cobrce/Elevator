using Elevator.Automation.IOPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DummyPlcPlugin.Plugin
{
    class DummyPoint : AbstractPoint
    {

        public DummyPoint()
        {
        }

        public DummyPoint(int byteIndex, PointType pointType = PointType.pBit) : base(byteIndex, pointType)
        {
        }

        public DummyPoint(int deviceIndex, int byteIndex, int bitIndex, PointType pointType = PointType.pBit) : base(deviceIndex, byteIndex, bitIndex, pointType)
        {
        }

        protected override string GetFormatted() => $"{ByteIndex:d3}";

        public override string RegexPattern => @"^(?<byte>[0-9]{1,3})$";
    }
}
