using Elevator.Automation;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

using Elevator.Automation.IOPoint;
using System.Reflection;
using System.IO;
using S7PROSIMLib;
using System;

namespace Step7PlcsimPlugin
{
    public class Plcsim : IPLC
    {
        S7ProSimClass sim = new S7ProSimClass();
        public Plcsim()
        {
        }

        public string Name { get; set; } = "Step7 Plcsim plugin for Elevator";

        public bool Connect()
        {
            try
            {
                sim.Connect();
                sim.SetScanMode(ScanModeConstants.ContinuousScan);
                if (sim.GetState() == "ERROR")
                    return false;
                return true;
            }
            catch { return false; }
        }

        public void Disconnect()
        {
            sim.Disconnect();
        }

        public int? Read(IPoint output)
        {
            object data = null;
            PointDataTypeConstants DataType = GetDataType(output);
            switch (output.Segment)
            {
                case MemorySegment.output:
                    sim.ReadOutputPoint(output.ByteIndex, output.BitIndex, DataType, ref data);
                    break;
                case MemorySegment.datablock:
                    sim.ReadDataBlockValue(output.DeviceIndex, output.ByteIndex, output.BitIndex, DataType, ref data);
                    break;
                case MemorySegment.memory:
                    sim.ReadFlagValue(output.ByteIndex, output.BitIndex, DataType, ref data);
                    break;
                default:
                    return null;
            }
            return ParseObject(data);
        }

        private static int? ParseObject(object data)
        {
            if (data is bool b)
                return b ? 1 : 0;
            else if (int.TryParse(data.ToString(), out int parsed))
                return parsed;
            else
                return null;
        }

        private static PointDataTypeConstants GetDataType(IPoint output)
        {
            PointDataTypeConstants type;
            switch (output.PointType)
            {
                case PointType.pByte:
                    type = PointDataTypeConstants.S7_Byte;
                    break;
                case PointType.pDword:
                    type = PointDataTypeConstants.S7_DoubleWord;
                    break;
                case PointType.pWord:
                    type = PointDataTypeConstants.S7_Word;
                    break;
                default:
                    type = PointDataTypeConstants.S7_Bit;
                    break;
            }
            return type;
        }

        public void Run()
        {
            sim.Continue();
        }

        public void Stop()
        {
            sim.Pause();
        }

        public void Write(IPoint input, int state)
        {
            Write(input, ToBoolArray(input.PointType, state));
        }

        private void Write(IPoint input, bool[] data)
        {

            for (int i = 0; i < data.Length; i++)
            {
                object o = data[i];
                switch (input.Segment)
                {
                    case MemorySegment.input:
                        sim.WriteInputPoint(input.ByteIndex, input.BitIndex, ref o);
                        break;
                    case MemorySegment.memory:
                        sim.WriteFlagValue(input.ByteIndex, input.BitIndex, ref o);
                        break;
                    case MemorySegment.datablock:
                        sim.WriteDataBlockValue(input.DeviceIndex, input.ByteIndex, input.BitIndex, ref o);
                        break;
                }
            }
        }

        private bool[] ToBoolArray(PointType pointType, int state)
        {
            int size;
            switch (pointType)
            {
                case PointType.pBit:
                    return new bool[] { (state & 1) == 1 };
                case PointType.pByte:
                    size = 8;
                    break;
                case PointType.pWord:
                    size = 16;
                    break;
                default:
                    size = 32;
                    break;
            }
            return ToBoolArray(size, state);
        }

        private bool[] ToBoolArray(int count, int state)
        {
            bool[] data = new bool[count];
            for (int i = 0; i < count; i++)
                data[i] = ((state >> (count - i - 1)) & 1) == 1;
            return data;

        }
    }
}