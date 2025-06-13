using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GridSettings gridSettings;
    public GridSettings GridSettings => gridSettings;

    private GridNode[,] gridNodes;
    public GridNode[,] GridNodes => gridNodes;

    public bool IsInitialized { get; private set; } = false;

    public AStarPathfinder aStarPathfinder;
    public PathFinder pathFinder; // in case you still want older pathfinder

    [SerializeField] private Color finalPathColor = Color.cyan;
    [SerializeField] private List<TerrainType> terrainTypes = new List<TerrainType>();

    public GridNode StartNode { get; private set; }
    public GridNode EndNode { get; private set; }

    private List<GridNode> finalPath = new();  // private field now, cleaner
    public List<GridNode> FinalPath => finalPath;


    private void Start()
    {
        InitializeGrid();

        // TEMP hardcoded start/end for testing, you can replace this later
        StartNode = gridNodes[1, 1];
        EndNode = gridNodes[8, 8];
        Vector2Int startIndex = GridNodeToIndex(StartNode);
        Vector2Int endIndex = GridNodeToIndex(EndNode);


        // Call your AStar after full grid is ready:
        if (aStarPathfinder != null)
            finalPath = aStarPathfinder.FindPath(StartNode.WorldPosition, EndNode.WorldPosition);
    }

    public Vector2Int GridNodeToIndex(GridNode node)
    {
        int x = Mathf.RoundToInt(node.WorldPosition.x / GridSettings.NodeSize);
        int y = GridSettings.UseXZPlane
            ? Mathf.RoundToInt(node.WorldPosition.z / GridSettings.NodeSize)
            : Mathf.RoundToInt(node.WorldPosition.y / GridSettings.NodeSize);

        return new Vector2Int(x, y);
    }


    public void InitializeGrid()
    {
        gridNodes = new GridNode[gridSettings.GridSizeX, gridSettings.GridSizeY];

        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < gridSettings.GridSizeY; y++)
            {
                Vector3 worldPos = gridSettings.UseXZPlane
                    ? new Vector3(x, 0, y) * gridSettings.NodeSize
                    : new Vector3(x, y, 0) * gridSettings.NodeSize;

                // You can randomize or hardcode terrain type
                TerrainType terrain = terrainTypes.Count > 0 ? terrainTypes[0] : new TerrainType();

                gridNodes[x, y] = new GridNode
                {
                    Name = $"Cell_{x}_{y}",
                    WorldPosition = worldPos,
                    Walkable = terrain.walkable,
                    Weight = terrain.movementCost,
                    TerrainType = terrain,
                    GizmoColor = terrain.gizmoColor
                };
            }
        }
        IsInitialized = true;
    }

    public GridNode GetNode(int x, int y)
    {
        if (x < 0 || x >= gridSettings.GridSizeX || y < 0 || y >= gridSettings.GridSizeY)
            throw new System.IndexOutOfRangeException("Grid node indices out of range.");
        return gridNodes[x, y];
    }

    public void SetWalkable(int x, int y, bool walkable)
    {
        gridNodes[x, y].Walkable = walkable;
    }

    public GridNode GetNodeFromWorldPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / gridSettings.NodeSize);
        int y = gridSettings.UseXZPlane
            ? Mathf.RoundToInt(position.z / gridSettings.NodeSize)
            : Mathf.RoundToInt(position.y / gridSettings.NodeSize);
        x = Mathf.Clamp(x, 0, gridSettings.GridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSettings.GridSizeY - 1);
        return GetNode(x, y);
    }

    public void SetFinalPath(List<GridNode> path)
    {
        finalPath = path;
    }

    private void OnDrawGizmos()
    {
        if (gridNodes == null || gridSettings == null) return;

        // Draw grid
        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < gridSettings.GridSizeY; y++)
            {
                GridNode node = gridNodes[x, y];
                Gizmos.color = node.Walkable ? Color.green : Color.red;
                Gizmos.DrawWireCube(node.WorldPosition, Vector3.one * gridSettings.NodeSize * 0.9f);
            }
        }

        // Draw final path
        if (finalPath != null && finalPath.Count > 1)
        {
            Gizmos.color = finalPathColor;
            for (int i = 0; i < finalPath.Count - 1; i++)
            {
                Gizmos.DrawLine(finalPath[i].WorldPosition, finalPath[i + 1].WorldPosition);
            }
        }
    }

    public Vector2Int GetGridIndexFromWorld(Vector3 worldPos)
{
    int x = Mathf.RoundToInt(worldPos.x / GridSettings.NodeSize);
    int y = Mathf.RoundToInt(worldPos.z / GridSettings.NodeSize);
    return new Vector2Int(x, y);
}

public bool IsRegionWalkable(int x, int y, int width, int height)
{
    for (int dx = 0; dx < width; dx++)
    {
        for (int dy = 0; dy < height; dy++)
        {
            if (!gridNodes[x + dx, y + dy].Walkable)
                return false;
        }
    }
    return true;
}

public void SetRegionWalkable(int x, int y, int width, int height, bool walkable)
{
    for (int dx = 0; dx < width; dx++)
    {
        for (int dy = 0; dy < height; dy++)
        {
            gridNodes[x + dx, y + dy].Walkable = walkable;
        }
    }
}

    // Custom editor button (optional quality of life for debugging)
    [CustomEditor(typeof(GridManager))]
    public class GridManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GridManager grid = (GridManager)target;
            if (GUILayout.Button("Initialize Grid"))
            {
                grid.InitializeGrid();
            }
        }
    }
}
