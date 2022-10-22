using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    public class EnergyMineFactory : MapObjectFactory
    {
        protected override MapObject CreateMapObjectSubClass(HexCoord position, Tribe tribe)
        {
            return new EnergyMine(position, tribe);
        }
    }
}
