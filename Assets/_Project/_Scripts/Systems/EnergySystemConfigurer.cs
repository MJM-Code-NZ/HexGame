using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    // Allows settings for the EnergySystem static class to be configured from UnityEditor
    public class EnergySystemConfigurer : MonoBehaviour
    {
        [SerializeField]
        private int cellEnergyCap = 10;  //Editor change only apply on startup

        private int minCellEnergyCap = 1;
        public void Init()
        {
            EnergySystem.CellEnergyCap = cellEnergyCap;
        }

        private void OnValidate()
        {
            cellEnergyCap = Math.Max(cellEnergyCap, minCellEnergyCap);
        }
    }
}
