using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace MJM.HG
{
    public class EntitySystem : GameSystem
    {
        public static event EventHandler<OnMapObjectEventArgs> OnCreateMapObject;

        private EnergyMineFactory _energyMineFactory; 

        public override void Init()
        {
            SetupEvents();

            CreateInitialMapObjects();
        }

        private void SetupEvents()
        {
            GameManager.OnTick += Tick;
        }
        
        private void CreateInitialMapObjects()
        {
            World _world = GameManager.Instance.WorldSystem.World;
            GameSettings _gamesettings = GameInfo.GameSettings;

            _energyMineFactory = new EnergyMineFactory();
            
            // Currently only set up to handle a single player.  Will change with start scene
            // If any other value is provided for player numbers just set an empty map object list - effectively zero players
            if (_gamesettings.NumberOfPlayers == 1)
            {
                // Initial energy mine position provided by GameInfo parameters check they are valid values before creating objects
                // Later will probably add option for random / automatic postioning

                int2 _offsetPositon = new int2(_gamesettings.Player1XPosiiton, _gamesettings.Player1YPosiiton);

                HexCoord position = HexCoordConversion.OffsetToHexCoord(_offsetPositon);

                if (_world.OnMap(position))
                {
                    //Currently a player is just an energy mine object
                    EnergyMine newEnergyMine;

                    newEnergyMine = (EnergyMine)_energyMineFactory.CreateMapObject(position, _world);

                    OnCreateMapObject?.Invoke(this, new OnMapObjectEventArgs { MapObject = newEnergyMine });                   
                }
                else
                {
                    Debug.Log("Player position outside world boundaries." + this);
                }
            }
            else
            {
                Debug.Log("Number of players not 1." + this);
            }
        }
       
        protected override void Tick(object sender, OnTickArgs eventArgs)
        {
           
        }

        public override void Quit()
        {
            GameManager.OnTick -= Tick;
        }
    }
}


