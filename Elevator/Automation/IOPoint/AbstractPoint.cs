using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Elevator.Automation.IOPoint
{
    public abstract class AbstractPoint : IPoint
    {

        public AbstractPoint()
        {

        }
        public AbstractPoint(int byteIndex, PointType pointType = PointType.pBit)
        {
            PointType = pointType;
            ByteIndex = byteIndex;
        }
        public AbstractPoint(int deviceIndex, int byteIndex, int bitIndex, PointType pointType = PointType.pBit)
        {
            PointType = pointType;
            DeviceIndex = deviceIndex;
            ByteIndex = byteIndex;
            BitIndex = bitIndex;
        }

        [JsonIgnore]
        public int DeviceIndex { get; set; }
        [JsonIgnore]
        public int ByteIndex { get; set; }
        [JsonIgnore]
        public int BitIndex { get; set; }

        public virtual string Formatted
        {
            get
            {
                return GetFormatted();
            }
            set
            {
                SetFormatted(value);
            }
        }

        protected virtual string GetFormatted()
        {
            return $"{DeviceIndex:d2}.{ByteIndex:d2}.{BitIndex:d2}";
        }

        protected virtual void SetFormatted(string value)
        {
            Match m = Regex.Match(value, RegexPattern);
            if (m.Success)
            {
                DeviceIndex = Parse(m, "device");
                ByteIndex = Parse(m, "byte");
                BitIndex = Parse(m, "bit");
            }
        }

        protected virtual int Parse(Match m, string group)
        {
            string value = m.Groups[group].Value;
            // a more generic parser, no need to override very often
            if (string.IsNullOrEmpty(value))
                return 0;

            return int.Parse(value);
        }

        public virtual string RegexPattern { get => @"^(?<device>[0-9]{1,2})\.(?<byte>[0-9]{1,2})\.(?<bit>)[0-9]{1,2}$"; }
        public virtual PointType PointType { get; set; }
        public override string ToString() => Formatted;
    }

}
