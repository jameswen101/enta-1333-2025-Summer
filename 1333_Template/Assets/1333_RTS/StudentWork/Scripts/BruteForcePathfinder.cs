using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DijkstraPathfinder : PathfindingAlgorithm
{
    public override List<GridNode> FindPath(GridManager gridManager, GridNode start, GridNode end, int unitWidth, int unitHeight)
    {
        var o = new SortedSet<(int, GridNode)>(Comparer<(int, GridNode)>.Create((a, b) => a.Item1 == b.Item1 ? a.Item2.GetHashCode().CompareTo(b.Item2.GetHashCode()) : a.Item1.CompareTo(b.Item1)));
        List<GridNode> openSet = new List<GridNode>();
        Dictionary<GridNode, int> costSoFar = new Dictionary<GridNode, int>();
        Dictionary<GridNode, GridNode> cameFrom = new Dictionary<GridNode, GridNode>();
        o.Add((0, start)); costSoFar[start] = 0; cameFrom[start] = start;
        openSet.Add(start);
        costSoFar[start] = 0;
        cameFrom[start] = start;
        while (openSet.Count > 0)
        {
            GridNode current = openSet[0];
            foreach (var node in openSet)
            {
                if (costSoFar[node] < costSoFar[current])
                {
                    current = node;
                }
            }
            if (current.Equals(end))
            {
                break;
            }
            openSet.Remove(current);
            foreach (GridNode neighbor in GetNeighbors(gridManager, current)) //GetNeighbors is a 2D array function? 
            {
                if (!IsAreaWalkable(gridManager, neighbor, unitWidth, unitHeight)) //IsAreaWalkable is a bool function
                continue;

                int newCost = costSoFar[current] + neighbor.Weight;

                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    costSoFar[neighbor] = newCost;
                    cameFrom[neighbor] = current;
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }

            }
        }

        List<GridNode> path = new List<GridNode>();
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
        GridNode startNode = gridManager.GetNodeFromWorldPosition(start);
        GridNode endNode = gridManager.GetNodeFromWorldPosition(end);
        return FindPath(gridManager, startNode, endNode, unitWidth, unitHeight);
    }

    private List<GridNode> GetNeighbors(GridManager gridManager, GridNode node) 
    { 
        List<GridNode > neighbors = new List<GridNode>();
        int gridSizeX = gridManager.GridSettings.GridSizeX;
        int gridSizeY = gridManager.GridSettings.GridSizeY;
        float nodeSize = gridManager.GridSettings.NodeSize;
        int nodeX = Mathf.RoundToInt(node.WorldPosition.x/nodeSize);
        int nodeY = Mathf.RoundToInt(node.WorldPosition.z/nodeSize);
        if (nodeY + 1 < gridSizeY) neighbors.Add(gridManager.GetNode(nodeX, nodeY + 1));
        if (nodeY - 1 >= 0) neighbors.Add(gridManager.GetNode(nodeX, nodeY - 1));
        if (nodeX + 1 < gridSizeX) neighbors.Add(gridManager.GetNode(nodeX + 1, nodeY));
        if (nodeX - 1 >= 0) neighbors.Add(gridManager.GetNode(nodeX - 1, nodeY));
        return neighbors;
    }

    private bool IsAreaWalkable(GridManager grid, GridNode node, int width, int height)
    {
        float nodeSize = grid.GridSettings.NodeSize;
        int x = Mathf.RoundToInt(node.WorldPosition.x/nodeSize);
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
