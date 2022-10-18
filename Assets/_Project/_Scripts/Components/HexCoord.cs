using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace MJM.HG
{
    // For the HexGame application - HexCoords are the primary hexagonal coordinate system and they use the cube hexgrid coordinate system.
    // The intention is that the application will use these cube coordinates in almost all situations.
    //
    // The exceptions are:
    //     Offset coordiantes used for unity tilemaps, the unity engine in general and cases where they are simpler
    // such as display to users and generating the world map layout.
    //     Partial coordinates (q, r) referred to as HexKeys used when hexcoordinates need to be the mey to a data collection - specifically
    // the key to the dictionary of all the hexes in the world  (Question: Just using the full 3 part coord as the key should also work.
    // Would that have performance implications?)
    //
    // Unity grid / tilemap swaps the x and y axis of offset coordinates in order to work with flat top hexes. This can be seen in the
    // Cell Swizzle YXZ option on the hex grid object. Together with the use of cube coordinates this may result in the
    // layout or direction of the axes for the coordinate systems not lining up with the standard approach found in other implementations.

    public struct HexCoord
    {
        public int q;
        public int r;
        public int s;

        public HexCoord(int q, int r, int s)
        {
            this.q = q;
            this.r = r;
            this.s = s;
            if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
        }

        // Should methods below be in a separate class?  Helper class?

        static public List<HexCoord> Directions = new List<HexCoord> {
            new HexCoord(1, 0, -1),
            new HexCoord(1, -1, 0),
            new HexCoord(0, -1, 1),
            new HexCoord(-1, 0, 1),
            new HexCoord(-1, 1, 0),
            new HexCoord(0, 1, -1)
        };

        static public HexCoord Direction(int direction)
        {
            return HexCoord.Directions[direction];
        }

        public HexCoord Add(HexCoord b)
        {
            return new HexCoord(q + b.q, r + b.r, s + b.s);
        }

        public HexCoord Neighbor(int direction)
        {
            return Add(Direction(direction));
        }
    }
}
