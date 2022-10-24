using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    public class PossibleEnergyTarget
    {
        private HexCell _hexCell;
        public HexCell HexCell { get { return _hexCell; } }

        private int _weighting;
        public int Weighting { get { return _weighting; } }

        // Class used by  EnergySystem for temporary processing of data in a list
        public PossibleEnergyTarget(HexCell hexCell, int weighting)
        {
            _hexCell = hexCell;
            _weighting = weighting;
        }
    }
}
