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
    [SerializeField] private ArmyPathfindingTester armyPathfindingTester;
    [SerializeField] private UnitInstance testUnit;
    [SerializeField] private ArmyComposition armyComposition;
    private TeamArmies AllArmies = new TeamArmies();
    [SerializeField] private CurrentTeamArmyManager currentTeamArmyManager;

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
        //startNode = unitManager.NodeSpawned(startMarker);
        unitManager.SpawnDummyUnit(endMarker);
        //endNode = unitManager.NodeSpawned(endMarker);
        armyPathfindingTester.Initialize();

        for (int i = 0; i < 4; i++)
        {
            AllArmies.Teams.Add(Instantiate(currentTeamArmyManager));

        }
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
        /*
        RandomizeAll();
        testUnit.transform.position = startMarker.transform.position;
        testUnit.SetTarget(endMarker.transform.position);
        */
        //change all terrains
        //for (int x = 0; x < gridmanager.gridsettings.gridsizex; x++)
        //{
        //    for (int y = 0; y < gridmanager.gridsettings.gridsizey; y++)
        //    {
        //        gridnode node = gridmanager.gridnodes[x, y];
        //        terraintype terraintype = gridmanager.terraintypes[random.range(0, gridmanager.terraintypes.count)];
        //        node.terraintype = terraintype;
        //        debug.log($"node ({gridmanager.startnode.worldposition.x}, {gridmanager.startnode.worldposition.z})'s terrain: {node.terraintype.terrainname}"); //how to increment the x/z values?
        //    }
        //}
        //change startnode
        //gridManager.StartNode = gridManager.gridNodes[Random.Range(0, gridManager.GridSettings.GridSizeX), Random.Range(0, gridManager.GridSettings.GridSizeY)];
        //gridManager.StartNode = gridManager.gridNodes[0,0];
        Debug.Log($"New StartNode: ({gridManager.StartNode.WorldPosition.x}, {gridManager.StartNode.WorldPosition.z})");
        //change endnode
        //gridManager.EndNode = gridManager.gridNodes[Random.Range(0, gridManager.GridSettings.GridSizeX), Random.Range(0, gridManager.GridSettings.GridSizeY)];
        //gridManager.EndNode = gridManager.gridNodes[9,9];
        Debug.Log($"New EndNode: ({gridManager.EndNode.WorldPosition.x}, {gridManager.EndNode.WorldPosition.z})");
        //call findpath
        testUnit.transform.position = startMarker.position;
        testUnit.SetTarget(endMarker.position);
        //TestPathfinderNoUnits();
    }

    private void TestPathfinderNoUnits() //no way to debug
    {
        var path = aStarPathfinder.FindPath(gridManager, startMarker.position, endMarker.position, 10, 10);
        string msg = $"Path found: {path.Count} steps. Start at {startMarker.position}, end at {endMarker.position}.";
        foreach (GridNode node in path)
        {
            msg += $"\n {node.WorldPosition}";
        }
        msg += $" > end at {endMarker.position}";
        Debug.Log(msg);
    }

    void Start()
    {
        //RandomizeAndPathFind();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        { 
            RandomizeAndPathFind();
            Debug.Log("Grid randomized");
        }
    }
}
