using System.Collections;
using System.Collections.Generic;
//using NUnit;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AStarPathfinder : PathfindingAlgorithm
{
    //public List<GridNode> FinalPath = new List<GridNode>();
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Color finalPathColor = Color.cyan;
    [SerializeField] private GameManager gameManager;
    public override List<GridNode> FindPath(GridManager gridManager, GridNode start, GridNode end, int unitWidth, int unitHeight)
    {
        //nodes to explore
        List<GridNode> openSet = new List<GridNode>(); //should we also have a closedSet?
        List<GridNode> closedSet = new List<GridNode>();
        Dictionary<GridNode, int> costSoFar = new Dictionary<GridNode, int>(); //gCost (starts at 0)
        Dictionary<GridNode, int> estimatedTotalCost = new Dictionary<GridNode, int>(); //fCost; costSoFar + Heuristic = estimatedTotalCost?
        Dictionary<GridNode, GridNode> cameFrom = new Dictionary<GridNode, GridNode>(); //path reconstruction
        openSet.Add(start);
        costSoFar[start] = 0;
        estimatedTotalCost[start] = Heuristic(start, end);
        cameFrom[start] = start; //start node has no parent or node preceding it

        Debug.Log($"Finding path from {start} to {end}");

        while (openSet.Count > 0) 
        {
            //find code with lowest f-score
            GridNode current = openSet[0];
            foreach (GridNode node in openSet) //does openSet only have 1 node?
            {
                if (estimatedTotalCost[node] < estimatedTotalCost[current] ||
   (estimatedTotalCost[node] == estimatedTotalCost[current] &&
    Heuristic(node, end) < Heuristic(current, end)))
                {
                    current = node;
                }
            }
            if (current.Equals(end)) //when end node reached, stop searching
            {
                break;
            }
            openSet.Remove(current); //remove current node from nodes to explore
            if (closedSet.Contains(current)) continue;
            closedSet.Add(current);
            //get neighbors
            var neighbors = GetNeighbors(gridManager, current);
                for (int i = 0; i < GetNeighbors(gridManager, current).Count; i++)
            {
                GridNode neighbor = neighbors[i];

                bool isWalkable = IsAreaWalkable(gridManager, neighbor, unitWidth, unitHeight);
                if (!isWalkable) //IsAreaWalkable is a bool function //misinterprets whether node is walkable or not
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

        List<GridNode> path = new List<GridNode>(); //Makes new list every time, otherwise old data can't be cleared
        GridNode pathNode = end;
        if (!cameFrom.ContainsKey(end))
            return path;
        while (!pathNode.Equals(start)) //not the 1st node
        {
            path.Add(pathNode); 
            pathNode = cameFrom[pathNode]; //key not found
        }
        path.Add(start);
        path.Reverse();
        return path;
    }

    public override List<GridNode> FindPath(GridManager gridManager, Vector3 start, Vector3 end, int unitWidth, int unitHeight)
    {
        GridNode startNode = FindClosestNode(gridManager, start);
        GridNode endNode = FindClosestNode(gridManager, end);
        return FindPath(gridManager, startNode, endNode, unitWidth, unitHeight);

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
                if (x + dx < 0 || x + dx >= grid.GridSettings.GridSizeX || y + dy < 0 || y + dy >= grid.GridSettings.GridSizeY)
                    return false;
                if (!grid.GetNode(x + dx, y + dy).Walkable)
                    return false;
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
