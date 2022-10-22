using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace MJM.HG
{
    public class EntitySystem
    {
        public static event EventHandler<OnMapObjectEventArgs> OnCreateMapObject;

        private EnergyMineFactory _energyMineFactory; 

        public void Initialize(World world, List<int2> playerPositionList)
        {
            //SetupEvents();

            _energyMineFactory = new EnergyMineFactory();

            CreateTribes(world, playerPositionList);
            
            //CreateInitialMapObjects(world, playerPositionList);
        }

        //private void SetupEvents()
        //{
        //    No events currently
        //}
        
        private void CreateTribes(World world, List<int2> playerPositionList)
        {
            for (int i = 0; i < playerPositionList.Count; i++)
            {
                Tribe _tribe = new Tribe(i);

                world.TribesList.Add(_tribe);

                CreateInitialMapObject(world, playerPositionList[i], _tribe);              
            }
        }

        private void CreateInitialMapObject(World world, int2 playerPosition, Tribe _tribe)
        {
            // World _world = GameManager.Instance.WorldSystem.World;
            //GameManager _gameManager = GameManager.Instance;
               
            HexCoord position = HexCoordConversion.OffsetToHexCoord(playerPosition);

            if (world.OnMap(position))
            {                   
                EnergyMine newEnergyMine;

                newEnergyMine = (EnergyMine)_energyMineFactory.CreateMapObject(position, world, _tribe);

                OnCreateMapObject?.Invoke(this, new OnMapObjectEventArgs { MapObject = newEnergyMine });                   
            }
            else
            {
                Debug.Log("Player position outside world boundaries." + this);
            }                      
        }

        public void Quit()
        {
            // This is still called by game manager and will be needed if class starts listening for events in the future
        }
    }
}


