using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJM.HG
{
    public static class EnergySystem
    {
        private static int _cellEnergyCap = 10; // Value can be overriden from EnergySystemConfigurer
        public static int CellEnergyCap 
        { 
            get { return _cellEnergyCap; }
            set { _cellEnergyCap = value; }
        }

        private static World _world;
        private static World World { get { return _world; } }

        public static event EventHandler<OnHexCellEventArgs> OnUpdateHexCell;

        

        private static int _maxEnergyDiff = 0;

        private static List<HexCell> _possibleEnergyTargets;
        
        // Energy updates are collected in a dictionary and processed later to avoid multiple cell update events per cell
        private static Dictionary<HexKey,int> _energyUpdates;  

        public static void ProcessTick(World world)
        {
            _world = world;

            if (_possibleEnergyTargets == null)
            {
                _possibleEnergyTargets = new List<HexCell>(6);
            }

            if (_energyUpdates == null)
            {
                _energyUpdates = new Dictionary<HexKey, int>();
            }
            else
            {
                if (_energyUpdates.Count > 0)
                {
                    _energyUpdates.Clear();
                }
            }

            World.EnergyActionCountdown--; // time manager rework

            if (World.EnergyActionCountdown == 0)
            {
                CalculateWorldEnergySpread();
                ProcessEnergyMines();
                UpdateWorldEnergy();

                World.EnergyActionCountdown = World.EnergyTickFrequency; // time manager rework
            }
        }

        private static void CalculateWorldEnergySpread()
        {
            foreach (KeyValuePair<HexKey, HexCell> keyValuePair in World.HexCells)
            {
                HexCell hexCell = keyValuePair.Value;

                if (hexCell.GroundType == GroundType.Standard)
                {
                    CalculateCellEnergySpread(hexCell);
                }
            }
        }

        // Energy can spread between cells if source cell energy is 2 > than target cell energy
        // 1 - Determine list of neighbor cells where energy could spread  with highest energy difference from source
        // 2 - If multiple equal targets select one randomly
        // 3 - Build a dictionary of energy updates.  Energy updates will actually be applied in later method
        private static void CalculateCellEnergySpread(HexCell hexCell)
        {
            _maxEnergyDiff = 0; // highest energy difference found so far

            _possibleEnergyTargets.Clear();

            // Build list of candidate target cells
            for (int direction = 0; direction < 6; direction++)
            {
                HexCoord neighbor = hexCell.Position.Neighbor(direction);

                HexCell neighborCell = World.LookupHexCell(neighbor);

                if (neighborCell.GroundType == GroundType.Standard)
                {
                    ProcessEnergyDifference(hexCell, neighborCell);                                
                }
            }

            if (_maxEnergyDiff > 0)
            {
                HexCell _energyTarget = DetermineEnergyTarget();  // energy can move - determine target

                StoreEnergyUpdate(hexCell, -1);
                StoreEnergyUpdate(_energyTarget, 1);

                // Debug.Log("Energy flow from " + hexCell.Position + " to " + energyTarget.Position);
            }
        }

        private static void ProcessEnergyDifference(HexCell hexCell, HexCell neighborCell)
        {
            int _energyDiff = hexCell.Energy - neighborCell.Energy;

            if (_energyDiff > 1) // energy could flow
            {
                if (_energyDiff > _maxEnergyDiff)
                {
                    // new highest energy diff
                    if (_possibleEnergyTargets.Count > 0)
                    {
                        _possibleEnergyTargets.Clear();
                    }

                    _possibleEnergyTargets.Add(neighborCell);
                    _maxEnergyDiff = _energyDiff;
                }
                else if (_energyDiff == _maxEnergyDiff)
                {                  
                    _possibleEnergyTargets.Add(neighborCell); // add to list of highest energy diff
                }
                else
                {
                    // lower energy diff - do nothing
                }
            }
        }

        private static HexCell DetermineEnergyTarget()
        {
            if (_possibleEnergyTargets.Count > 1)
            {               
                return _possibleEnergyTargets[RandomHelper.RandomRange(0, _possibleEnergyTargets.Count - 1)];
            }
            else
            {
                return _possibleEnergyTargets[0];
            }
        }

        private static void StoreEnergyUpdate(HexCell hexCell, int amount)
        {
            HexKey _hexKey = HexCoordConversion.HexCoordToHexKey(hexCell.Position);
            
            if (_energyUpdates.ContainsKey(_hexKey))
            {
                _energyUpdates[_hexKey] += amount;
            }
            else
            {
                _energyUpdates.Add(_hexKey, amount);
            }
        }

        // Currently only handles energy mines extracting energy from world.
        // May expand to other types of map objects that have energy based action impacting world in future
        // May also change in future to approach where mines extract to energy tank rather than releasing energy directly to world      
        private static void ProcessEnergyMines()
        {
            foreach (KeyValuePair<HexKey, MapObject> keyValuePair in World.MapObjects)
            {
                MapObject mapObject = keyValuePair.Value;

                if (mapObject is EnergyMine)
                {
                    HexCell hexCell = World.HexCells[HexCoordConversion.HexCoordToHexKey(mapObject.Position)];

                    StoreEnergyUpdate(hexCell, 1);
                }
            }
        }

        private static void UpdateWorldEnergy()
        {
            foreach (KeyValuePair<HexKey, int> keyValuePair in _energyUpdates)
            {
                HexCell hexCell = World.HexCells[keyValuePair.Key];

                if (keyValuePair.Value != 0)  // update value can be zero as a result of two updates canceling each other out - do nothing         
                {
                    hexCell.Energy += keyValuePair.Value;

                    if (hexCell.Energy > CellEnergyCap)
                    {
                        hexCell.Energy = CellEnergyCap;

                        Debug.Log("Energy cap breached cell id:" + hexCell.Position);
                    }

                    OnUpdateHexCell?.Invoke("static World Energy System", new OnHexCellEventArgs { Hexcell = hexCell });
                }
            }

            _energyUpdates.Clear();
        }
        
    }
}
