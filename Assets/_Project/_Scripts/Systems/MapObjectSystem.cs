using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace MJM.HG
{
    public class MapObjectSystem
    {
        public static event EventHandler<OnMapObjectEventArgs> OnCreateMapObject;

        private EnergyMineFactory _energyMineFactory; 

        public void Init(int players)
        {
            //SetupEvents();

            CreateInitialMapObjects(players);
        }

        //private void SetupEvents()
        //{
        //    No events currently
        //}
        
        private void CreateInitialMapObjects(int players)
        {
            World _world = GameManager.Instance.WorldSystem.World;
            GameManager _gameManager = GameManager.Instance;

            _energyMineFactory = new EnergyMineFactory();
            
            // Currently only set up to handle a single player.  Will change with start scene
            // If any other value is provided for player numbers just set an empty map object list - effectively zero players
            if (players == 1)
            {
                // Initial energy mine position provided by GameManager parameters check they are valid values before creating objects
                // Later will probably add option for random / automatic postioning

                int2 _offsetPositon = new int2(_gameManager.PlayerParameters.Player1XPosiiton, _gameManager.PlayerParameters.Player1YPosiiton);

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

        public void Quit()
        {
            // This is still called by game manager and will be needed if class starts listening for events in the future
        }
    }
}


