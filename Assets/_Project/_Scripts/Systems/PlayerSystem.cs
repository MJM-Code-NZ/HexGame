using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace MJM.HG
{
    public static class PlayerSystem
    { 
        // Calculate how many players can fit in world
        public static int CalcMaxPlayers(int worldSize)
        {
            int _playersPerEdge = CalcPlayersPerEdge(worldSize);

            int _maxPlayers = _playersPerEdge * _playersPerEdge;

            return _maxPlayers;
        }
        
        // Calculate how many players can fit on one edge / axes of world
        public static int CalcPlayersPerEdge(int worldSize)
        {
            PlayerParameters _param = GameManager.Instance.PlayerParameters;

            // First calculate how many players fit on one edge
            int _widthAvailable = 2 * worldSize + 1;
            int _widthRequired;
            int i = 1;
            do
            {
                i++;
                _widthRequired = i * _param.SpacePerPlayer + (i - 1) * _param.SpaceBetweenPlayers;
            }
            while (_widthRequired <= _widthAvailable);

            int _playersPerSide = i - 1;

            return _playersPerSide;
        }

        public static List<int2> DeterminePlayerLocations(World world, int players)
        {
            List<int2> _playerPositionList = new List<int2>(players);

            // This is a different calculation than the possible players per edge calculated elsewhere in this class
            int _actualPlayersPerEdge = (int)Math.Ceiling(Math.Sqrt(players));

            int _playerPositionsAvailable = _actualPlayersPerEdge * _actualPlayersPerEdge;

            int _unPlacedPlayers = players;

            for (int x = 1; x <= _actualPlayersPerEdge; x++)
                for (int y = 1; y <= _actualPlayersPerEdge; y++)
                {
                    int rnd = RandomHelper.RandomRange(1, _playerPositionsAvailable);

                    if (rnd <= _unPlacedPlayers)
                    {
                        _playerPositionList.Add(DeterminePosition(x, y, _actualPlayersPerEdge, world.Width));

                        _unPlacedPlayers--;
                    }
                    else
                    {
                        // Skip this positon
                    }

                    _playerPositionsAvailable--;
               }

            return _playerPositionList;
        }

        public static int2 DeterminePosition(int x, int y, int actualPlayersPerEdge, int width)
        {
            PlayerParameters _param = GameManager.Instance.PlayerParameters;
    
            int _requiredGaps = _param.SpaceBetweenPlayers * (actualPlayersPerEdge - 1);

            int _remainingSpace = width - _requiredGaps;

            int _spawnWidth = _remainingSpace / actualPlayersPerEdge;

            int _freeSpace = _remainingSpace - actualPlayersPerEdge * _spawnWidth;

            int _corner = (width - _freeSpace) / 2;

            int _minSpawnX = -_corner + _spawnWidth * (x - 1) + _param.SpaceBetweenPlayers * (x - 1);

            int _maxSpawnX = -_corner + _spawnWidth * (x - 1) + _param.SpaceBetweenPlayers * (x - 1) + _spawnWidth - 1;

            int _posX = RandomHelper.RandomRange(_minSpawnX, _maxSpawnX);

            int _minSpawnY = -_corner + _spawnWidth * (y - 1) + _param.SpaceBetweenPlayers * (y - 1);

            int _maxSpawnY = -_corner + _spawnWidth * (y - 1) + _param.SpaceBetweenPlayers * (y - 1) + _spawnWidth - 1;

            int _posY = RandomHelper.RandomRange(_minSpawnY, _maxSpawnY);
          
            return new int2(_posX, _posY);
        }

    }
}


