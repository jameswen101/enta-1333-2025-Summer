using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private PathFinder pathfinder;
    [SerializeField] private AStarPathfinder aStarPathfinder;
    //[SerializeField] private 
    public Transform startMarker;
    //[SerializeField] private
    public Transform endMarker;
    public GridNode startNode;
    public GridNode endNode;
    [SerializeField] private LineRenderer pathLine;

    [Header("Randomization Settings")]
    [SerializeField] private float markerHeight = 0.5f;
    [SerializeField] private TerrainType[] availableTerrainTypes;
    [SerializeField] private int currentSeed = 0;

    // Start is called before the first frame update

    private void Awake()
    {
        if (!ValidateReferences())
        {
            Debug.LogError("GridTest: Missing required references. Please assign all required components in the inspector.");
            enabled = false;
            return;
        }
        gridManager.InitializeGrid();
        unitManager.SpawnDummyUnit(startMarker);
        //save GridNode made as a result of SpawnDummyUnit
        startNode = unitManager.NodeSpawned(startMarker);
        unitManager.SpawnDummyUnit(endMarker);
        endNode = unitManager.NodeSpawned(endMarker);
        //RandomizeAndPathFind();
    }

    private bool ValidateReferences() //add GridTest serializefield private variables here?
    {
        if (!gridManager || !pathfinder || !startMarker || !endMarker || !pathLine)
        {
            return false;
        }
        return true;
    }

    public void RandomizeAndPathFind()
    {
        //change all terrains
        for (int x = 0; x < gridManager.GridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < gridManager.GridSettings.GridSizeY; y++)
            {
                GridNode node = gridManager.gridNodes[x, y];
                TerrainType terrainType = gridManager.TerrainTypes[Random.Range(0, gridManager.TerrainTypes.Count)];
                node.TerrainType = terrainType;
                Debug.Log($"Node ({gridManager.StartNode.WorldPosition.x}, {gridManager.StartNode.WorldPosition.z})'s terrain: {node.TerrainType.terrainName}");
            }
        }
        //change startnode
        gridManager.StartNode = gridManager.gridNodes[Random.Range(0, gridManager.GridSettings.GridSizeX), Random.Range(0, gridManager.GridSettings.GridSizeY)];
        Debug.Log($"New StartNode: ({gridManager.StartNode.WorldPosition.x}, {gridManager.StartNode.WorldPosition.z}");
        //change endnode
        gridManager.EndNode = gridManager.gridNodes[Random.Range(0, gridManager.GridSettings.GridSizeX), Random.Range(0, gridManager.GridSettings.GridSizeY)];
        Debug.Log($"New EndNode: ({gridManager.EndNode.WorldPosition.x}, {gridManager.EndNode.WorldPosition.z}");
        //call findpath
        aStarPathfinder.FindPath(gridManager, gridManager.StartNode, gridManager.EndNode, 10, 10);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        { 
            RandomizeAndPathFind();
            Debug.Log("Grid randomized");
        }
    }
}
