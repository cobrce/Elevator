﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elevator.Automation.IOPoint
{
    public interface IPoint
    {
        int DeviceIndex { get; set; }
        int ByteIndex { get; set; }
        int BitIndex { get; set; }
        string Formatted { get; set; }
        string RegexPattern { get; }
        PointType PointType { get; set; }
        IPoint Clone();
        MemorySegment Segment { get; set; }

    }
    public enum PointType
    {
        pBit,
        pByte,
        pWord,
        pDword,
        pOther
    }

    public enum MemorySegment
    {
        input,
        output,
        memory,
        datablock,
        timer,
        counter,
        register
    }
}
