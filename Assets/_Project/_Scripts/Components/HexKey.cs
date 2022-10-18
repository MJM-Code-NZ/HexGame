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
    }
}
