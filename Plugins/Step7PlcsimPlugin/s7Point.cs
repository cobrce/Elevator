using Elevator.Automation.IOPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Step7PlcsimPlugin
{
    class S7Point : AbstractPoint
    {
        public S7Point(int byteIndex, int bitIndex, PointType pointType = PointType.pBit, MemorySegment segment = MemorySegment.output) : base(0, byteIndex, bitIndex, pointType)
        {

        }

        public S7Point()
        {

        }
        
        // overridden to support DB
        public override MemorySegment Segment
        {
            get => (DeviceIndex > 0) ? MemorySegment.datablock : base.Segment;
            set => base.Segment = value;
        }
        protected override string GetFormatted() => ((DeviceIndex > 0) ? $"DB{DeviceIndex}." : "") + $"{ByteIndex}.{BitIndex:d1}";
        public override string RegexPattern => @"^(DB(?<device>[0-9]{1,2})\.)?(?<byte>[0-9]{1,2})\.(?<bit>[0-7]{1})";// @" ^ (?<byte>[0-9]{1,2})\.(?<bit>[0-7]{1})$";
    }
}
