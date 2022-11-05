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

        private static int _totalSpreadWeighting = 0;

        private static List<PossibleEnergyTarget> _possibleEnergyTargets;

        private static Dictionary<HexKey, EnergyFlow> _energyFlows;
        private static Dictionary<HexKey, EnergyUpdate> _energyUpdates;


        // The process of energy updates currently works as follows:
        // Energy mines just add energy to the hex the mine is on each tick.
        // Energy flow is calculated by looking at each hex and determining if there is a neighbor hex that energy can flow to.
        // Currently energy flows are limited to 1 unit of energy moving from 1 hex to another each tick.
        // Additionally each hex can not be the source for more than one energy flow per tick and cannot be the destination for 
        // more than one energy flow per tick.
        // Implementation of these rules is a 3 stage process:
        // 1 - Loop through each hex, identify potential energy flow out of that hex and store that potential flow in a dictionary.
        // This structure will be keyed by destination hex and also contain the source hex, a flag indicating whether the potential 
        // flow has been canceled and a linked list of any other potential flows to that hex.
        // 2 - Build an update dictionary of the actual energy changes per hex.  This is done by processing the data structure from step 1.
        // Ignoring canceled entries and resolving issues with multiple potential flows per hex.  Additonally at this point add energy changes 
        // due to energy mines to the update structure.
        // 3 - Process the update dictionary making the actual changes to the energy values in the hex objects and invoking the hex changed event
        // to trigger the energy change per cell being reflected in the rendering opf the hex.

        public static void ProcessWorldTick(World world)
        {
            _world = world;

            if (_possibleEnergyTargets == null)
                _possibleEnergyTargets = new List<PossibleEnergyTarget>(6);

            if (_energyFlows == null)
                _energyFlows = new Dictionary<HexKey, EnergyFlow>();
            else
            {
                if (_energyFlows.Count > 0)
                    _energyFlows.Clear();
            }

            if (_energyUpdates == null)
                _energyUpdates = new Dictionary<HexKey, EnergyUpdate>();
            else
            {
                if (_energyUpdates.Count > 0)
                    _energyUpdates.Clear();
            }

            CalculateWorldEnergySpread();
            ResolveEnergyFlowConflicts();
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
            _totalSpreadWeighting = 0;

            _possibleEnergyTargets.Clear();

            // Build list of candidate target cells
            for (int direction = 0; direction < 6; direction++)
            {
                // Performance change to how neighbor cells are found
                //HexCoord neighbor = hexCell.Position.Neighbor(direction);

                //HexCell neighborCell = World.LookupHexCell(neighbor);

                HexCell neighborCell = hexCell.LookupNeighbor(direction);

                if (neighborCell.GroundType == GroundType.Standard)
                    ProcessEnergyDifference(hexCell, neighborCell);
            }

            if(_possibleEnergyTargets.Count > 0)            
            {
                HexCell _energyTarget = DetermineEnergyTarget();  // energy can move - determine target

                StoreEnergyFlow(_energyTarget, hexCell);
            }
        }

        // Selecting where energy will flow uses a weighting system rather than automatically selecting the neighbor with
        // the lowest energy to avoid some deadlock scenarios that were occurring between players
        private static void ProcessEnergyDifference(HexCell hexCell, HexCell neighborCell)
        {
            int _energyDiff = hexCell.Energy - neighborCell.Energy;

            if (_energyDiff < 2) return; // energy difference is too small;

            // Determine if energy flow is blocked by another energyOwner
            if (!EnergyOwnerMatch(hexCell, neighborCell)) return;
           
            _possibleEnergyTargets.Add(new PossibleEnergyTarget(neighborCell, _energyDiff - 1));
            
            _totalSpreadWeighting += (_energyDiff - 1);         
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
                // Performance change to how neighbor cells are found
                //HexCoord neighbor2Position = neighborCell.Position.Neighbor(direction);

                //HexCell neighbor2Cell = World.LookupHexCell(neighbor2Position);

                HexCell neighbor2Cell = neighborCell.LookupNeighbor(direction);

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
            int randomSelection = RandomHelper.RandomRange(1, _totalSpreadWeighting);

            for (int i = 0; i < 6; i++)
            {
                if (randomSelection > _possibleEnergyTargets[i].Weighting)
                {
                    randomSelection -= _possibleEnergyTargets[i].Weighting;
                }
                else
                {
                    return _possibleEnergyTargets[i].HexCell;
                }
            }

            Debug.Log("No target selected by energy difference weighting");

            return null;
        }

        private static bool StoreEnergyFlow(HexCell targetHex, HexCell sourceHex)
        {
            HexKey _hexKey = HexCoordConversion.HexCoordToHexKey(targetHex.Position);

            EnergyFlow _energyFlow;
           
            if (_energyFlows.TryGetValue(_hexKey, out _energyFlow))
            {
                if (_energyFlow.OtherSources == null)
                    _energyFlow.OtherSources = new List<HexCell>();

                _energyFlow.OtherSources.Add(sourceHex);

                _energyFlows[_hexKey] = _energyFlow;
            }
            else
            {
                if (targetHex.EnergyOwner == null)
                {
                    // when adding a new hex with no current energy owner to the update list it is also necessary to check 
                    // for update entries for neighbor hexes and resolve any conflicts

                    bool updateOK = HandleNeighborEnergyFlows(targetHex, sourceHex.EnergyOwner);

                    if (!updateOK) return false; //If conflict is found do not perform this update

                }
                // Add new energyflow entry
                _energyFlows.Add(_hexKey, new EnergyFlow(sourceHex));
            }

            return true;
        }

        private static bool HandleNeighborEnergyFlows(HexCell hexCell, Tribe newEnergyOwner)
        {
            for (int direction = 0; direction < 6; direction++)
            {
                // Performance change to how neighbor cells are found
                //HexCoord _neighborPosition = hexCell.Position.Neighbor(direction);

                //HexCell _neighborCell = World.LookupHexCell(_neighborPosition);

                HexCell _neighborCell = hexCell.LookupNeighbor(direction);

                if (_neighborCell.GroundType == GroundType.Standard
                    && _neighborCell.EnergyOwner == null)
                {
                    if (!HandleNeighborFlow(_neighborCell, newEnergyOwner))
                        return false;
                }
            }
            return true; // no conflict found
        }

        private static bool HandleNeighborFlow(HexCell neighborCell, Tribe newEnergyOwner)
        {
            //Potential conflict if neighbor cell is in the flows dictionary
            EnergyFlow _energyFlow;

            HexKey _neighborPosition = HexCoordConversion.HexCoordToHexKey(neighborCell.Position);

            if (_energyFlows.TryGetValue(_neighborPosition, out _energyFlow))
            {
                if (_energyFlow.Cancel)
                {
                    // if energyUpdates to this hex have already been cancelled return false as additional flows to the hex will also not be allowed
                    return false;
                }

                if (_energyFlow.Source.EnergyOwner != newEnergyOwner)
                {
                    // There is a conflict - mark the conflicting energy flow as canceled and return false indicating not to proceed
                    // with the energy flow currently being checked

                    _energyFlow.Cancel = true;

                    _energyFlows[_neighborPosition] = _energyFlow;

                    return false;
                }
            }
            return true;
        }

        private static void ResolveEnergyFlowConflicts()
        {
            foreach (KeyValuePair<HexKey, EnergyFlow> keyValuePair in _energyFlows)
            {
                HexKey _targetHexKey = keyValuePair.Key;
                EnergyFlow _energyFlow = keyValuePair.Value;
                int _numberOfSources;

                if (!_energyFlow.Cancel)
                {
                    if (_energyFlow.OtherSources == null)
                    {
                        _numberOfSources = 1;
                    }
                    else
                    { 
                        _numberOfSources = _energyFlow.OtherSources.Count + 1;
                    }

                    int _selectedSource = RandomHelper.RandomRange(1, _numberOfSources);

                    //SetEnergyUpdate(_targetHexKey, _energyFlow, _selectedSource);

                    HexKey _sourceHexKey;

                    if (_selectedSource == 1)
                    {
                        SetEnergyUpdate(_targetHexKey, 1, _energyFlow.Source.EnergyOwner);

                        _sourceHexKey = HexCoordConversion.HexCoordToHexKey(_energyFlow.Source.Position);

                        SetEnergyUpdate(_sourceHexKey, -1, _energyFlow.Source.EnergyOwner);
                    }
                    else
                    {
                        SetEnergyUpdate(_targetHexKey, 1, _energyFlow.OtherSources[_selectedSource - 2].EnergyOwner);

                        _sourceHexKey = HexCoordConversion.HexCoordToHexKey(_energyFlow.OtherSources[_selectedSource - 2].Position);

                        SetEnergyUpdate(_sourceHexKey, -1, _energyFlow.OtherSources[_selectedSource - 2].EnergyOwner);
                    } 
                }
            }
        }

        private static void SetEnergyUpdate(HexKey target, int amount, Tribe energyOwner)
        {
            EnergyUpdate _energyUpdate = new EnergyUpdate(amount, energyOwner);
           
            if (!_energyUpdates.TryAdd(target, _energyUpdate))
                _energyUpdates[target].Amount += amount;          
        }

        // Currently only handles energy mines extracting energy from world.
        // May expand to other types of map objects that have energy based action impacting world in future
        // May also change in future to approach where mines extract to energy tank rather than releasing energy directly to world      
        private static void ProcessEnergyMines()
        {
            foreach (KeyValuePair<HexKey, MapObject> keyValuePair in World.MapObjects)
            {
                MapObject mapObject = keyValuePair.Value;

                //EnergyFlow _energyFlow = new EnergyFlow(mapObject.Position)

                if (mapObject is EnergyMine)
                    SetEnergyUpdate(HexCoordConversion.HexCoordToHexKey(mapObject.Position), 1, mapObject.Tribe);
            }
        }

        private static void UpdateWorldEnergy()
        {
            foreach (KeyValuePair<HexKey, EnergyUpdate> keyValuePair in _energyUpdates)
            {
                EnergyUpdate _energyUpdate = keyValuePair.Value; 
                
                HexCell hexCell = World.HexCells[keyValuePair.Key];
            
                if (_energyUpdate.Amount != 0)  // update value can be zero as a result of two updates canceling each other out - do nothing         
                {
                    hexCell.Energy += _energyUpdate.Amount;

                    if (hexCell.Energy > CellEnergyCap)
                    {
                        hexCell.Energy = CellEnergyCap;

                        //Debug.Log("Energy cap breached cell id:" + hexCell.Position);
                    }

                    if (hexCell.EnergyOwner == null)
                        hexCell.EnergyOwner = _energyUpdate.EnergyOwner;

                    OnUpdateHexCell?.Invoke("static World Energy System", new OnHexCellEventArgs { Hexcell = hexCell });
                }
            }

            _energyUpdates.Clear();
        }
        
    }
}
