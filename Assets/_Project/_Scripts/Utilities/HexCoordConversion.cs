using Unity.Mathematics;

namespace MJM.HG
{
    public static class HexCoordConversion
    {
        // In this case offset refers to offset coordinates for a hex grid of flat top hexes (with a positive offset?)
        public static HexCoord OffsetToHexCoord(int2 offset)
        {
            int q = offset.y;
            int r = offset.x - (offset.y - (offset.y & 1)) / 2;
            int s = -q - r;

            return new HexCoord(q, r, s);
        }

        public static int2 HexCoordToOffset(HexCoord cube)
        {
            int2 offset;
         
            offset.x = cube.r + (cube.q - (cube.q & 1)) / 2;
            offset.y = cube.q;

            return offset;
        }

        public static HexCoord HexKeyToHexCoord(HexKey hexkey)
        {
            HexCoord hexCoord;

            hexCoord.q = hexkey.q;
            hexCoord.r = hexkey.r;
            hexCoord.s = -hexkey.q - hexkey.r;

            return hexCoord;
        }

        public static HexKey HexCoordToHexKey(HexCoord hexCoord)
        {
            HexKey hexKey;

            hexKey.q = hexCoord.q;
            hexKey.r = hexCoord.r;    

            return hexKey;
        }

        public static HexKey OffsetToHexKey(int2 offset)
        {
            HexKey hexKey;

            hexKey.q = offset.y;
            hexKey.r = offset.x - (offset.y - (offset.y & 1)) / 2;

            return hexKey;
        }

        public static int2 HexKeyToOffset(HexKey hexKey)
        {
            int2 offset;

            offset.x = hexKey.r + (hexKey.q - (hexKey.q & 1)) / 2;
            offset.y = hexKey.q;

            return offset;
        }
    }
}
