using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace MJM.HG
{
    public class World 
    {
        // Size,width etc. are based off standard hexes and do not account for edge hexes.
        // Add 1 to size and 2 to width to get values inclusive of edge hexes
        private int _size;
        public int Size
        {
            get {return _size; }
        }

        public int Width => 2 * _size + 1;
        public int Area => Width * Width;

        public int SizeWithEdge => _size + 1;
        public int WidthWithEdge => 2 * SizeWithEdge + 1;
        public int AreaWithEdge => WidthWithEdge * WidthWithEdge;

        private Dictionary<HexKey, HexCell> _hexCells;
        public Dictionary<HexKey, HexCell> HexCells
        {
            get { return _hexCells; }
        }

        private Dictionary<HexKey, MapObject> _mapObjects;
        public Dictionary<HexKey, MapObject> MapObjects
        {
            get { return _mapObjects; }
        }

        // How many ticks between energy actions  //move out to some sort of time or energy class
        public int EnergyTickFrequency = 5;

        // Counter of how many ticks until next energy action
        public int EnergyActionCountdown = 5;

        public World(int size)
        {
            _size = size;

            _hexCells = new Dictionary<HexKey, HexCell>(AreaWithEdge);

            _mapObjects = new Dictionary<HexKey, MapObject>();  // initial size of dictionary GameInfo.GameSettings.NumberOfPlayers Come back on this when rework players
        }

        public void AddHexCell(HexCoord position, HexCell hexCell)
        {
            _hexCells.Add(HexCoordConversion.HexCoordToHexKey(position), hexCell);
        }

        public void AddMapObject(HexCoord position, MapObject mapObject)       
        {
            _mapObjects.Add(HexCoordConversion.HexCoordToHexKey(position), mapObject);
        }

        public HexCell LookupHexCell(HexCoord position)
        {
            HexCell hexCell;

            if (_hexCells.TryGetValue(HexCoordConversion.HexCoordToHexKey(position), out hexCell))
            {
                return hexCell;
            }
            else
            {
                Debug.Log("Attempt to access hex not in dictionary: " + position);

                return null;
            }
        }

        public bool OnMap(HexCoord position)
        {
            HexCell hexCell = LookupHexCell(position);

            if (hexCell == null || hexCell.GroundType != GroundType.Standard)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


    }
}
