using System;
using System.Collections.Generic;

namespace MJM.HG
{
    public class Tribe
    {
        private static int _lastColor = -1;

        private static int _maxColor = 5;
            
            private int _id;
        public int Id { get { return _id; } }

        private int _energyColor;
        public int EnergyColor { get { return _energyColor; } }

        private List<MapObject> _mapobjectList;
        public List<MapObject> MapObjectList { get { return _mapobjectList; } }

        public Tribe(int id)
        {
            _id = id;

            _lastColor++;

            if (_lastColor > _maxColor)
            {
                _lastColor = 0;
            }

            _energyColor = _lastColor;

            _mapobjectList = new List<MapObject>();

        }
    }
}

