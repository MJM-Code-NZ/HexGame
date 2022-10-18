using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace MJM.HG
{
    public class WorldSystem : GameSystem
    {
        public static event EventHandler<OnWorldEventArgs> OnUpdateWorldRender;

        private World _world;
        public World World
        {
            get { return _world; }
        }

        public new void Init()
        {
            SetupEvents();
            GenerateWorldMap();
        }

        private void SetupEvents()
        {
            GameManager.OnTick += Tick;
        }

        private void GenerateWorldMap()
        {
            _world = new World(GameInfo.GameSettings.WorldSize); // In time world size will be passed into scene as a parameter

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

        protected override void Tick(object sender, OnTickArgs eventArgs)
        {           
            EnergySystem.ProcessTick(World); // time manager rework
        }

        public override void Quit()
        {
            GameManager.OnTick -= Tick;
        }
    }
}
