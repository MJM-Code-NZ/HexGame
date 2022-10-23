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
        public struct EnergyUpdate { public int amount; public Tribe energyOwner; }
        private static Dictionary<HexKey, EnergyUpdate> _energyUpdates;  

        public static void ProcessWorldTick(World world)
        {
            _world = world;

            if (_possibleEnergyTargets == null)
            {
                _possibleEnergyTargets = new List<HexCell>(6);
            }

            if (_energyUpdates == null)
            {
                _energyUpdates = new Dictionary<HexKey, EnergyUpdate>();
            }
            else
            {
                if (_energyUpdates.Count > 0)
                {
                    _energyUpdates.Clear();
                }
            }
          
            CalculateWorldEnergySpread();
            ProcessEnergyMines();
            UpdateWorldEnergy();            
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

                if (StoreEnergyUpdate(_energyTarget, 1, hexCell.EnergyOwner))
                {
                    // Only subtract energy from source hex if addition of energy to target hex succeeded
                    StoreEnergyUpdate(hexCell, -1);
                    // Debug.Log("Energy flow from " + hexCell.Position + " to " + energyTarget.Position);
                }
            }
        }

        private static void ProcessEnergyDifference(HexCell hexCell, HexCell neighborCell)
        {
            int _energyDiff = hexCell.Energy - neighborCell.Energy;

            if (_energyDiff < 2) return; // energy difference is too small;

            // Determine if energy flow is blocked by another energyOwner
            if (!EnergyOwnerMatch(hexCell, neighborCell)) return;

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

        private static bool EnergyOwnerMatch(HexCell hexCell, HexCell neighborCell)
        {
            if (hexCell.EnergyOwner == neighborCell.EnergyOwner) return true;

            // In theory a neighbor cell should always match or be null but this check is included for safety / completeness
            if (neighborCell.EnergyOwner != null)
            {
                Debug.Log($"neighbor no match and not null {neighborCell}");
                return false;
            }

            // Determine if any of the neighbors of neighbor cell do not match hexcells energy
            for (int direction = 0; direction < 6; direction++)
            {
                HexCoord neighbor2Position = neighborCell.Position.Neighbor(direction);

                HexCell neighbor2Cell = World.LookupHexCell(neighbor2Position);

                if (neighbor2Cell.GroundType == GroundType.Standard
                    && neighbor2Cell.EnergyOwner != null
                    && neighbor2Cell.EnergyOwner != hexCell.EnergyOwner)
                {
                    return false;
                }
            }

            return true;                
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

        private static bool StoreEnergyUpdate(HexCell hexCell, int amount, Tribe newEnergyOwner = null)
        {
            HexKey _hexKey = HexCoordConversion.HexCoordToHexKey(hexCell.Position);

            EnergyUpdate _energyUpdate;
       
            if (_energyUpdates.TryGetValue(_hexKey, out _energyUpdate))
            {
                // if hex already has an update entry just change the value
                _energyUpdate.amount += amount;

                _energyUpdates[_hexKey] = _energyUpdate;
            }
            else
            {
                _energyUpdate.amount = amount;

                if (hexCell.EnergyOwner != null)
                {                   
                    _energyUpdate.energyOwner = hexCell.EnergyOwner;
                }
                else
                {
                    // when adding a new hex with no current energy owner to the update list it is also necessary to check 
                    // for update entries for neighbor hexs and resolve any conflicts

                    bool updateOK = HandleNeighborEnergyUpdates(hexCell, newEnergyOwner);

                    if (!updateOK) return false; //If conflict is found do not perform this update

                    _energyUpdate.energyOwner = newEnergyOwner;
                }

                _energyUpdates.Add(_hexKey, _energyUpdate);
            }

            return true;
        }

        private static bool HandleNeighborEnergyUpdates(HexCell hexCell, Tribe newEnergyOwner)
        {
            for (int direction = 0; direction < 6; direction++)
            {
                HexCoord neighborPosition = hexCell.Position.Neighbor(direction);

                HexCell neighborCell = World.LookupHexCell(neighborPosition);

                if (neighborCell.GroundType == GroundType.Standard
                    && neighborCell.EnergyOwner == null)                   
                {
                    //Potential conflict if neighbor cell is in the update dictionary
                    EnergyUpdate _energyUpdate;

                    if (_energyUpdates.TryGetValue(HexCoordConversion.HexCoordToHexKey(neighborPosition), out _energyUpdate))
                    {
                        if (_energyUpdate.energyOwner != newEnergyOwner)
                        {
                            // There is a conflict - need to reverse the update entry
                            HandleReversingUpdate(neighborCell);

                            return false;
                        }
                        
                    }
                        
                }
            }

            return true; // no conflict found
        }

        private static void HandleReversingUpdate(HexCell hexCell)
        {
            // For now this will do nothing - this essentially leads to energy being lost from the world

            // The approach to energy flow has become complex. The most likely resolution to reversing the updates is 
            // to start storing the energy source on the update and then logging reversals to a cancelled update dictionary
            // UpdateWorldEnergy will then have to process both the energy updates and update cancel dictionaries
            // as it updates the actual energy on the hexcells. Possibly also easier to handle energy mine updates completely separately from flows.
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

                    StoreEnergyUpdate(hexCell, 1, mapObject.Tribe);
                }
            }
        }

        private static void UpdateWorldEnergy()
        {
            foreach (KeyValuePair<HexKey, EnergyUpdate> keyValuePair in _energyUpdates)
            {
                HexCell hexCell = World.HexCells[keyValuePair.Key];

                if (keyValuePair.Value.amount != 0)  // update value can be zero as a result of two updates canceling each other out - do nothing         
                {
                    hexCell.Energy += keyValuePair.Value.amount;

                    if (hexCell.Energy > CellEnergyCap)
                    {
                        hexCell.Energy = CellEnergyCap;

                        Debug.Log("Energy cap breached cell id:" + hexCell.Position);
                    }

                    if (hexCell.Energy != 0 && hexCell.EnergyOwner == null)
                    {
                        hexCell.EnergyOwner = keyValuePair.Value.energyOwner;
                    }

                    OnUpdateHexCell?.Invoke("static World Energy System", new OnHexCellEventArgs { Hexcell = hexCell });
                }
            }

            _energyUpdates.Clear();
        }
        
    }
}
