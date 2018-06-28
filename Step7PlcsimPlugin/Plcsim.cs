using Elevator.Automation;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using S7PROSIMLib;
using Elevator.Automation.IOPoint;
using System.Reflection;
using System.IO;

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
            object o = null;
            sim.ReadOutputPoint(output.ByteIndex, output.BitIndex, type, ref o);

            if (int.TryParse(o.ToString(), out int parsed))
                return parsed;
            else
                return null;
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
            object o = state;
            sim.WriteInputPoint(input.ByteIndex, input.BitIndex, ref o);
        }
    }
}