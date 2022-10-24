using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    public class EnergyUpdate
    {
        private int _amount;
        public int Amount { get { return _amount; } set { _amount = value; } }

        private Tribe _energyOwner;
        public Tribe EnergyOwner { get { return _energyOwner; } }

        // Class used by  EnergySystem for temporary processing of data in a dictionary
        public EnergyUpdate(int amount, Tribe energyOwner)
        {
            _amount = amount;
            _energyOwner = energyOwner;
        }
    }
}
