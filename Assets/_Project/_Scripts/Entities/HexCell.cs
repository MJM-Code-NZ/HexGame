using System;
using System.Collections.Generic;
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

        // Added to address neighbor lookup performance
        private List<HexCell> _neighborList;
        public List<HexCell> NeighborList
        {
            get { return _neighborList; }     
        }

        private int _energy;
        public int Energy
        {
            get { return _energy; }
            set { _energy = value; }
        }

        private Tribe _energyOwner;
        public Tribe EnergyOwner
        {
            get { return _energyOwner; }
            set { _energyOwner = value; }
        }

        // Hexcells are created using cube hex coords it is the responsibility of the object calling
        // the constructor to provide cube hex coordinates
        public HexCell(HexCoord position, GroundType groundType)
        {
            _position = position;

            _groundType = groundType;

            if (_groundType == GroundType.Standard)
            {
                _neighborList = new List<HexCell>(6);
            }
            else
            {
                _neighborList = null;
            }
            _energy = 0;

            _energyOwner = null;
        }

        public HexCell LookupNeighbor(int direction)
        {
            if (_groundType == GroundType.Standard)
            {
                return _neighborList[direction];
            }
            else
            {
                return null;
            }
        }
    }
}
