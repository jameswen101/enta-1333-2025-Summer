using System.Collections;
using System.Collections.Generic;
using NUnit;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AStarPathfinder : PathfindingAlgorithm
{
    //public List<GridNode> FinalPath = new List<GridNode>();
    List<GridNode> path = new List<GridNode>();
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Color finalPathColor = Color.cyan;
    [SerializeField] private GameManager gameManager;
    public override List<GridNode> FindPath(GridManager gridManager, GridNode start, GridNode end, int unitWidth, int unitHeight)
    {
        List<GridNode> openSet = new List<GridNode>(); //should we also have a closedSet?
        Dictionary<GridNode, int> costSoFar = new Dictionary<GridNode, int>(); //gCost (starts at 0)
        Dictionary<GridNode, int> estimatedTotalCost = new Dictionary<GridNode, int>(); //fCost; costSoFar + Heuristic = estimatedTotalCost?
        Dictionary<GridNode, GridNode> cameFrom = new Dictionary<GridNode, GridNode>(); 
        openSet.Add(start);
        costSoFar[start] = 0;
        estimatedTotalCost[start] = Heuristic(start, end);
        cameFrom[start] = start;

        while (openSet.Count > 0) 
        {
            GridNode current = openSet[0];
            foreach (GridNode node in openSet)
            {
                if (estimatedTotalCost[node] < estimatedTotalCost[current])
                {
                    current = node;
                }
            }
            if (current.Equals(end))
            {
                break;
            }
            openSet.Remove(current);
                for (int i = 0; i < GetNeighbors(gridManager, current).Count; i++)
            {
                GridNode neighbor = GetNeighbors(gridManager, current)[i];
                if (!IsAreaWalkable(gridManager, neighbor, unitWidth, unitHeight)) //IsAreaWalkable is a bool function
                    continue;

                int newCost = costSoFar[current] + neighbor.Weight;

                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    costSoFar[neighbor] = newCost;
                    estimatedTotalCost[neighbor] = newCost + Heuristic(neighbor, end); //fCost = gCost + hCost
                    cameFrom[neighbor] = current;
                    neighbor.gCost = newCost;
                    neighbor.hCost = Heuristic(neighbor, end);
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }

            }
        }
        
        GridNode pathNode = end;
        if (cameFrom.ContainsKey(end))
            return path;
        while (!pathNode.Equals(start))
        {
            path.Add(pathNode);
            pathNode = cameFrom[pathNode];
        }
        path.Add(start);
        path.Reverse();
        return path;
    }

    public override List<GridNode> FindPath(GridManager gridManager, Vector3 start, Vector3 end, int unitWidth, int unitHeight)
    {
        //add a* code here
        return FindPath(gridManager, start, end, 1, 1);
    }

    private List<GridNode> GetNeighbors(GridManager gridManager, GridNode node)
    {
        List<GridNode> neighbors = new List<GridNode>();
        int gridSizeX = gridManager.GridSettings.GridSizeX;
        int gridSizeY = gridManager.GridSettings.GridSizeY;
        float nodeSize = gridManager.GridSettings.NodeSize;
        int nodeX = Mathf.RoundToInt(node.WorldPosition.x / nodeSize);
        int nodeY = Mathf.RoundToInt(node.WorldPosition.z / nodeSize);
        if (nodeY + 1 < gridSizeY) neighbors.Add(gridManager.GetNode(nodeX, nodeY + 1));
        if (nodeY - 1 >= 0) neighbors.Add(gridManager.GetNode(nodeX, nodeY - 1));
        if (nodeX + 1 < gridSizeX) neighbors.Add(gridManager.GetNode(nodeX + 1, nodeY));
        if (nodeX - 1 >= 0) neighbors.Add(gridManager.GetNode(nodeX - 1, nodeY));
        return neighbors;
    }

    private bool IsAreaWalkable(GridManager grid, GridNode node, int width, int height)
    {
        float nodeSize = grid.GridSettings.NodeSize;
        int x = Mathf.RoundToInt(node.WorldPosition.x / nodeSize);
        int y = Mathf.RoundToInt(node.WorldPosition.z / nodeSize);
        for (int dx = 0; dx < width; dx++)
        {
            for (int dy = 0; dy < height; dy++)
            {
                if (x + dx < 0 || y + dy < 0 || x + dx >= grid.GridSettings.GridSizeX || y + dy >= grid.GridSettings.GridSizeY)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private GridNode FindClosestNode(GridManager gridManager, Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / gridManager.GridSettings.NodeSize);
        int y = Mathf.RoundToInt(position.z / gridManager.GridSettings.NodeSize);
        return gridManager.GetNode(x, y);
    }

    private int Heuristic(GridNode a, GridNode b)
    {
        float dx = Mathf.Abs(a.WorldPosition.x - b.WorldPosition.x);
        float dz = Mathf.Abs(a.WorldPosition.z - b.WorldPosition.z);
        return Mathf.RoundToInt(dx + dz);
    }


    public void OnDrawGizmos()
    {
        /*
        List<GridNode> FinalPath = FindPath(gridManager, start, end, int unitWidth, int unitHeight);
        if (FinalPath != null && FinalPath.Count > 1) //FinalPath nodes > 1?
        {
            Gizmos.color = finalPathColor;
            for (int i = 0; i < FinalPath.Count - 1; i++)
            {
                Gizmos.DrawLine(FinalPath[i].WorldPosition, FinalPath[i + 1].WorldPosition);
                Debug.Log($"Node visited: ({FinalPath[i].WorldPosition.x}, {FinalPath[i].WorldPosition.z})"); //make sure Debug.Log ends once destination reached
                //How to display gCost, fCost, and hCost in Debug.Log?
            }
        }
        */
        if (path == null || path.Count < 2)
        {
            Debug.LogError("Path not established");
            return;
        }
        if (path != null)
        {
            Gizmos.color = finalPathColor;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i].WorldPosition, path[i + 1].WorldPosition);
                Debug.Log($"Node visited: ({path[i].WorldPosition.x}, {path[i].WorldPosition.z}), gCost: {path[i].gCost}, hCost: {path[i].hCost}, fCost: {path[i].fCost}");
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gridManager.StartNode = gridManager.gridNodes[Random.Range(0, gridManager.GridSettings.GridSizeX), Random.Range(0, gridManager.GridSettings.GridSizeY)];
        //gameManager.startNode; 
        gridManager.EndNode = gridManager.gridNodes[Random.Range(0, gridManager.GridSettings.GridSizeX), Random.Range(0, gridManager.GridSettings.GridSizeY)];
        //gameManager.endNode; 
        path = FindPath(gridManager, gridManager.StartNode, gridManager.EndNode, 10, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
