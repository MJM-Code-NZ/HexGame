using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    public class EnergyFlow
    {
        private HexCell _source;
        public HexCell Source { get { return _source; } }

        private bool _cancel;
        public bool Cancel { get { return _cancel; } set { _cancel = value; } }

        private List<HexCell> _otherSources; 
        public List<HexCell> OtherSources { get { return _otherSources; } set { _otherSources = value; } }

        // Class used by  EnergySystem for temporary processing of data in a dictionary
        public EnergyFlow(HexCell hexCell)
        {
            _source = hexCell;
            _cancel = false;
            _otherSources = null;
        }
    }
}
