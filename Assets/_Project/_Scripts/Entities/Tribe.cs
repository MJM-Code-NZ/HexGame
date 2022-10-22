using System;
using System.Collections.Generic;

namespace MJM.HG
{
    public class Tribe
    {
        private int _id;
        public int Id { get { return _id; } }

        private int _energyColour;
        public int EnergyColour { get { return _energyColour; } }

        private List<MapObject> _mapobjectList;
        public List<MapObject> MapObjectList;


        public Tribe(int id)
        {
            _id = id;

            _energyColour = RandomHelper.RandomRange(0, 5);
        }
    }
}

