using System;

namespace MJM.HG
{
    public abstract class MapObject
    {
        private HexCoord _position;
        public HexCoord Position { get { return _position; } }


        private Tribe _tribe;
        public Tribe Tribe { get { return _tribe; } }

        public MapObject(HexCoord position, Tribe tribe)
        {
            _position = position;

            _tribe = tribe;
        }
    }
}

