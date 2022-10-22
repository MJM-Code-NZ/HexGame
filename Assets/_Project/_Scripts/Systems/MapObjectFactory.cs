using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    // Creating map object requires base level processing both before and after the subclass specific processing.
    // Therefore two methods have been created:
    // abstract CreateMapObject - contains the base level logic and calls the second method at the appropriate point
    // protected CreateMapObjectSubClass - is intended to be overidden for each map object type to actually create the 
    // correct subclass object.
    public abstract class MapObjectFactory
    {
        public MapObject CreateMapObject(HexCoord position, World world, Tribe tribe)
        {
            if (!world.OnMap(position))
            {
                Debug.Log("Attempt to create energy mine off map " + position + this);
                return null;
            }

            MapObject mapObject = CreateMapObjectSubClass(position, tribe);

            world.AddMapObject(position, mapObject);

            return mapObject;
        }

        protected virtual MapObject CreateMapObjectSubClass(HexCoord position, Tribe tribe)
        {
            return null;
        }
    }
}
