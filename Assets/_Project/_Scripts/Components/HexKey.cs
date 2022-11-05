using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace MJM.HG
{
    // HexKey takes the first 2 parts of hex cude coords.
    // This is used as the key for collections such as dictionaries working with hex coordinates
    public struct HexKey
    {
        public int q;
        public int r;

        public HexKey(int q, int r)
        {
            this.q = q;
            this.r = r;
        }

        public HexKey(HexCoord hexCoord) : this(hexCoord.q, hexCoord.r)
        {

        }

        // Don't really see why this is necessary but documentation seems to recommend overriding Equals when overriding hashcode
        // but suspect the default equals would be fine in this case
        public override bool Equals(Object obj)
        {
            if (!(obj is HexKey)) return false;

            HexKey p = (HexKey)obj;
            return q == p.q & r == p.r;
        }

        public override int GetHashCode()
        {
            uint h = 0x811c9dc5;
            h = (h ^ (uint)q) * 0x01000193;
            h = (h ^ (uint)q) * 0x01000193;
            h = (h ^ (uint)r) * 0x01000193;
            return (int)h;
        }

        //public override int GetHashCode()
        //{
        //    return ShiftAndWrap(q.GetHashCode(), 2) ^ r.GetHashCode();
        //}

        //private int ShiftAndWrap(int value, int positions)
        //{
        //    positions = positions & 0x1F;

        //    // Save the existing bit pattern, but interpret it as an unsigned integer.
        //    uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
        //    // Preserve the bits to be discarded.
        //    uint wrapped = number >> (32 - positions);
        //    // Shift and wrap the discarded bits.
        //    return BitConverter.ToInt32(BitConverter.GetBytes((number << positions) | wrapped), 0);
        //}
    }
}
