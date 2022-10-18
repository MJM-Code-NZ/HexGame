using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace MJM.HG
{
    public class WorldSystem
    {
        public static event EventHandler<OnWorldEventArgs> OnUpdateWorldRender;

        private World _world;
        public World World
        {
            get { return _world; }
        }

        public void Init(int worldSize, int players)
        {
            SetupEvents();
            GenerateWorldMap(worldSize, players);
        }

        private void SetupEvents()
        {
            //TimeManager.OnWorldTick += WorldTick;
            WorldTimer.OnWorldTick += WorldTick;
        }

        private void GenerateWorldMap(int worldSize, int players)
        {
            if (worldSize < 1)
            {
                Debug.Log("World size is too small " + worldSize + this);
            }
            
            _world = new World(worldSize, players); 

            for (int x = -World.SizeWithEdge; x <= World.SizeWithEdge; x++)
            {
                for (int y = -World.SizeWithEdge; y <= World.SizeWithEdge; y++)
                {
                    GenerateHexCell(x, y);
                }
            }

            OnUpdateWorldRender?.Invoke(this, new OnWorldEventArgs { World = World });
        }

        private void GenerateHexCell(int x, int y)
        {
            GroundType groundType = DetermineGroundType(x, y);
            
            HexCoord position = HexCoordConversion.OffsetToHexCoord(new int2(x, y));
         
            HexCell hexCell = new HexCell(position, groundType);

            World.AddHexCell(position, hexCell);
        }

        private GroundType DetermineGroundType(int x, int y)
        {           
            // Give map solid edges
            if (x <= -World.SizeWithEdge 
                || x >= World.SizeWithEdge
                || y <= -World.SizeWithEdge 
                || y >= World.SizeWithEdge)
            {
                return AdjustCornerEdgeGroundType(x, y);
            }
            else
            {
                return GroundType.Standard;
            }
        }

        private GroundType AdjustCornerEdgeGroundType(int x, int y)
        {
            // Extra logic to hide unnecessay corner edges not need due to hex alignment
            if (y == -World.SizeWithEdge || y == World.SizeWithEdge)
            {
                if (World.SizeWithEdge % 2 == 0)
                {
                    // Even world size so corners to be hidden are at bottom
                    if (x == -World.SizeWithEdge)
                    {
                        return  GroundType.None;
                    }
                }
                else
                {
                    // Odd world size so corners to be hidden are at top
                    if (x == World.SizeWithEdge)
                    {
                        return GroundType.None;
                    }
                }
            }
            return GroundType.Edge;         
        }

        protected void WorldTick(object sender, OnWorldTickArgs eventArgs)
        {           
            EnergySystem.ProcessWorldTick(World);
        }

        public void Quit()
        {
            //TimeManager.OnWorldTick -= WorldTick;
            WorldTimer.OnWorldTick -= WorldTick;
        }
    }
}
