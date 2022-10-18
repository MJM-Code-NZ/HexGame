using System;
using Unity.Mathematics;

namespace MJM.HG
{
    public class HexCell
    {
       
        private HexCoord _position;
        public HexCoord Position { get { return _position; } }

        private GroundType _groundType;
        public GroundType GroundType 
        { 
            get { return _groundType; }
            set { _groundType = value; } 
        }

        private int _energy;
        public int Energy
        {
            get { return _energy; }
            set { _energy = value; }
        }

        // Hexcells are created using cube hex coords it is the responsibility of the object calling
        // the constructor to provide cube hex coordinates
        public HexCell(HexCoord position, GroundType groundType)
        {
            _position = position;

            _groundType = groundType;

            _energy = 0;
        }       
    }
}
