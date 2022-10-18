using System;

namespace MJM.HG
{
    public abstract class MapObject
    {
        private HexCoord _position;
        public HexCoord Position
        {
            get { return _position; }
        }

        public MapObject(HexCoord position)
        {
            _position = position;
        }
    }
}

