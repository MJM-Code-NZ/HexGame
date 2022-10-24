using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;
using TMPro;

namespace MJM.HG
{
    public class WorldRender : MonoBehaviour
    {
        private Grid _hexGrid;

        private Tilemap _hexTilemap;
       
        [SerializeField] private Tile _standardGround;
        [SerializeField] private Tile _edgeGround;

        // Tiles are serialized as a list as a work around to being unable to serialize dictionaries
        // this data will be moved to a dictionary in a method in this script
        [SerializeField] private List<Tile> _energyTilesList;
        //private Dictionary<int, Tile> _energyTiles;
 
        [SerializeField]
        private GameObject _hexGridTextPrefab;
            
        // These two serialized lists are a solution to the problem of not being able to use
        // SerializeField to build a dictionary of map object prefabs through linking prefabs on the unity editor
        [SerializeField] private List<string>  _mapObjectNames;
        [SerializeField] private List<GameObject> _mapObjectPrefabsList;
        private Dictionary<string, GameObject> _mapObjectPrefabs;
             
        private GameObject _mapObjectsGameObject;

        private Dictionary<HexKey, GameObject> _hexTextRenderData;
        private Dictionary<HexKey, GameObject> _mapObjectsRenderData;

        [SerializeField]
        private HexGridText _hexGridText = HexGridText.None;
        [SerializeField]
        private int _energyDisplayCap = 5;

        // Awake is called on object creation
        void Awake()
        {           
            SetupEvents();
            SetupTilemapResources();
            SetupEntityResources();
        }

        private void SetupEvents()
        {
            WorldSystem.OnUpdateWorldRender += UpdateWorldRender;
            EntitySystem.OnCreateMapObject += CreateMapObjectRenderData;
            EnergySystem.OnUpdateHexCell += UpdateHexCellRender;          
        }

        private void SetupTilemapResources()
        {
            _hexGrid = GameObject.Find("HexGrid").GetComponent<Grid>();
            _hexTilemap = GameObject.Find("HexTilemap").GetComponent<Tilemap>();

            //_energyTiles = new Dictionary<int, Tile>
            //{
            //    [0] = null, // at this time there is a separate no energy tile outside this dictionary
            //    [1] = _energyTilesList[0],
            //    [2] = _energyTilesList[1],
            //    [3] = _energyTilesList[2],
            //    [4] = _energyTilesList[3],
            //    [5] = _energyTilesList[4],
            //};
        }

        private void SetupEntityResources()
        {
            _mapObjectsGameObject = GameObject.Find("MapObjects");

            _mapObjectsRenderData = new Dictionary<HexKey, GameObject>();

            // Creating dictionary of the subclasses of map object and the corresponding prefabs
            if (_mapObjectNames.Count == _mapObjectPrefabsList.Count)
            {
                _mapObjectPrefabs = new Dictionary<string, GameObject>();

                for ( int i = 0; i < _mapObjectNames.Count; i++)
                {
                    _mapObjectPrefabs.Add(_mapObjectNames[i], _mapObjectPrefabsList[i]);
                }
            }
            else
            {
                Debug.Log("Map Object Lists do not match length. Check serialized properties in unity editor" + this);
            }
        }

        void OnDisable()
        {
            WorldSystem.OnUpdateWorldRender -= UpdateWorldRender;
            EntitySystem.OnCreateMapObject -= CreateMapObjectRenderData;
            EnergySystem.OnUpdateHexCell -= UpdateHexCellRender;
        }

        private void UpdateWorldRender(object sender, OnWorldEventArgs eventArgs)
        // Currently only called on initial world map creation. After this creation updates are triggered at the individual hex cell level 
        {
            foreach (KeyValuePair<HexKey, HexCell> keyValuePair in eventArgs.World.HexCells)
            {
                HexCell hexCell = keyValuePair.Value;

                SetTile(hexCell);
            }

            if (_hexGridText != HexGridText.None) // texts are required
            {              
                if (_hexTextRenderData == null) // texts have not already been created
                {                 
                    CreateHexTexts(eventArgs.World);
                }
            }
        }

        private void SetTile(HexCell hexCell)
        {
            int2 positionOffset = HexCoordConversion.HexCoordToOffset(hexCell.Position);

            Vector3Int tilemapPosition = new Vector3Int(positionOffset.x, positionOffset.y, 0);

            Tile _tile = SelectTile(hexCell);

            _hexTilemap.SetTile(tilemapPosition, _tile);
        }

        private Tile SelectTile(HexCell hexCell)
        {
            if (hexCell.GroundType == GroundType.None) return null;
            
            if (hexCell.GroundType == GroundType.Edge) return _edgeGround;

            if (hexCell.Energy == 0) return _standardGround;

            int _cappedEnergy = Math.Min(hexCell.Energy, _energyDisplayCap);

            int _listIndex = hexCell.EnergyOwner.EnergyColor * _energyDisplayCap + _cappedEnergy - 1;

            return _energyTilesList[_listIndex];               
        }

        private void CreateHexTexts(World world)
        // Create text objects over each hex - currently used to provide more info for debugging purposes
        // Currently only works if text options are switched on in render settings at startup
        // Currently cannot change text options mid game
        {
            _hexTextRenderData = new Dictionary<HexKey, GameObject>(world.HexCells.Count);

            GameObject textGameObject;

            foreach (KeyValuePair<HexKey, HexCell> keyValuePair in world.HexCells)
            {
                HexCell hexCell = keyValuePair.Value;
            
                if (hexCell.GroundType == GroundType.Standard)
                {                   
                    Vector3 position = GridToWorld(HexCoordConversion.HexCoordToOffset(hexCell.Position));
                    
                    textGameObject = Instantiate(
                        _hexGridTextPrefab,
                        position,
                        Quaternion.identity,
                        _hexGrid.transform
                        );

                    _hexTextRenderData[HexCoordConversion.HexCoordToHexKey(hexCell.Position)] = textGameObject;

                    if (_hexGridText == HexGridText.PositionOffset)
                    {
                        UpdateHexPositionTextOffset(hexCell);
                    }

                    if (_hexGridText == HexGridText.PositionCube)
                    {
                        UpdateHexPositionTextCube(hexCell);
                    }

                    if (_hexGridText == HexGridText.Tribe)
                    {
                        UpdateHexTribe(hexCell);
                    }

                    // at this stage no need to update value of energy text across all hexes as all energy starts at 0 (the default text string)
                    // updates to energy values are handled by event call for individual hexes rather than entire world
                }
            }
        }
       
        private void UpdateHexPositionTextOffset(HexCell hexCell)
        {
            HexKey _hexKey = HexCoordConversion.HexCoordToHexKey(hexCell.Position);

            int2 _offsetPosition = HexCoordConversion.HexCoordToOffset(hexCell.Position);

            _hexTextRenderData[_hexKey].GetComponent<TextMeshPro>().text = _offsetPosition.y + ", " + _offsetPosition.x;
        }

        private void UpdateHexPositionTextCube(HexCell hexCell)
        {
            HexKey _hexKey = HexCoordConversion.HexCoordToHexKey(hexCell.Position);

            _hexTextRenderData[_hexKey].GetComponent<TextMeshPro>().text 
                = hexCell.Position.q + ", " + hexCell.Position.r + ", " + hexCell.Position.s;
        }

        private void UpdateHexEnergyText(HexCell hexCell)
        {
            HexKey _hexkey = HexCoordConversion.HexCoordToHexKey(hexCell.Position);

            //if (hexCell.Energy <= _energyDisplayCap)
            //{
                _hexTextRenderData[_hexkey].GetComponent<TextMeshPro>().text = hexCell.Energy.ToString();
            //}
            //else
            //{
            //    _hexTextRenderData[_hexkey].GetComponent<TextMeshPro>().text = _energyDisplayCap.ToString();
            //}
        }

        private void UpdateHexTribe(HexCell hexCell)
        // Could be updated to event format if there is ever a need to update hex positions
        {
            HexKey _hexKey = HexCoordConversion.HexCoordToHexKey(hexCell.Position);

            if (hexCell.EnergyOwner == null)
            {
                _hexTextRenderData[_hexKey].GetComponent<TextMeshPro>().text = null;
            }
            else
            {
                _hexTextRenderData[_hexKey].GetComponent<TextMeshPro>().text
                    = hexCell.EnergyOwner.Id.ToString(); ;
            }
        }

        private void UpdateHexCellRender(object sender, OnHexCellEventArgs eventArgs)
        // Currently handles changes to energy level in cell
        // Update tile displayed in hex based on energy level
        // Update energy text if it is selected to be rendered
        {
            HexCell hexCell = eventArgs.Hexcell;

            SetTile(hexCell);

            if (_hexGridText == HexGridText.Energy)
            {
                UpdateHexEnergyText(hexCell);
            }

            else if (_hexGridText == HexGridText.Tribe)
            {
                UpdateHexTribe(hexCell);
            }
        }

        private void CreateMapObjectRenderData(object sender, OnMapObjectEventArgs eventArgs)
        {
            MapObject mapObject = eventArgs.MapObject;

            Vector3 position = GridToWorld(HexCoordConversion.HexCoordToOffset(mapObject.Position));

            // Determine which subclass of map object is being handled
            // so that the corresponding prefab can be instantiated from the map object prefab dictionary

            Type t = mapObject.GetType();

            GameObject _gameObject = Instantiate(
                _mapObjectPrefabs[t.Name],
                position,
                Quaternion.identity,
                _mapObjectsGameObject.transform
                );

            _mapObjectsRenderData[HexCoordConversion.HexCoordToHexKey(mapObject.Position)] = _gameObject;
        }

        private Vector3 GridToWorld(int2 position)
        {
            Vector3 worldPosition = _hexGrid.CellToWorld(new Vector3Int(position.x, position.y, 0));

            return worldPosition;
        }
    }
}
