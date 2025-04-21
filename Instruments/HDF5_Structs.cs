using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ush4.Instruments
{
    public class HDF5_Structs
    {
        public unsafe struct SummaryValue
        {
            public Models.Borders.enSetPointType EnSetPointType;
            public Double SetValue;
            public Double MeasuredValue;
            public char* Units;
        }

        public struct Point
        {
            public Double X;
            public Double Y;
        }

        public static int FieldOffset(Type type, String field)
        {
            int t = Marshal.OffsetOf(type, field).ToInt32();
            return t;
        }

        public static int SizeOfStruct(Type type)
        {
            return Marshal.SizeOf(type);
        }
    }
}
