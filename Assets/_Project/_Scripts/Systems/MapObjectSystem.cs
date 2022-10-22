using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace MJM.HG
{
    //public class MapObjectSystem
    //{
    //    public static event EventHandler<OnMapObjectEventArgs> OnCreateMapObject;

    //    private EnergyMineFactory _energyMineFactory; 

    //    public void Initialize(World world, List<int2> playerPositionList)
    //    {
    //        //SetupEvents();

    //        CreateInitialMapObjects(world, playerPositionList);
    //    }

    //    //private void SetupEvents()
    //    //{
    //    //    No events currently
    //    //}
        
    //    private void CreateInitialMapObjects(World world, List<int2> playerPositionList)
    //    {
    //        // World _world = GameManager.Instance.WorldSystem.World;
    //        GameManager _gameManager = GameManager.Instance;

    //        _energyMineFactory = new EnergyMineFactory();
            
    //        foreach (int2 _offsetPosition in playerPositionList)
    //        {
                
    //            //Might use this later
    //            //int2 _offsetPositon = new int2(_gameManager.PlayerParameters.Player1XPosiiton, _gameManager.PlayerParameters.Player1YPosiiton);

    //            HexCoord position = HexCoordConversion.OffsetToHexCoord(_offsetPosition);

    //            if (world.OnMap(position))
    //            {
    //                //Currently a player is just an energy mine object
    //                EnergyMine newEnergyMine;

    //                newEnergyMine = (EnergyMine)_energyMineFactory.CreateMapObject(position, world);

    //                OnCreateMapObject?.Invoke(this, new OnMapObjectEventArgs { MapObject = newEnergyMine });                   
    //            }
    //            else
    //            {
    //                Debug.Log("Player position outside world boundaries." + this);
    //            }
    //        }           
    //    }

    //    public void Quit()
    //    {
    //        // This is still called by game manager and will be needed if class starts listening for events in the future
    //    }
    //}
}


