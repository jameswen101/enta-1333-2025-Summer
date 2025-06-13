using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    public List<GridNode> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        // Convert world positions to GridNodes
        GridNode startNode = gridManager.GetNodeFromWorldPosition(startWorldPos);
        GridNode endNode = gridManager.GetNodeFromWorldPosition(endWorldPos);

        // Safety check
        if (startNode.Equals(null) || endNode.Equals(null) || !startNode.Walkable || !endNode.Walkable)
        {
            Debug.LogWarning("Invalid start or end node.");
            return null;
        }

        // A* data structures
        var openSet = new List<GridNode> { startNode };
        var closedSet = new HashSet<GridNode>();
        var cameFrom = new Dictionary<GridNode, GridNode>();
        var gScore = new Dictionary<GridNode, int> { [startNode] = 0 };
        var fScore = new Dictionary<GridNode, int> { [startNode] = Heuristic(startNode, endNode) };

        while (openSet.Count > 0)
        {
            // Select node with lowest fScore
            GridNode current = GetLowestFScoreNode(openSet, fScore);

            if (current.Equals(endNode))
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (GridNode neighbor in GetNeighbors(current))
            {
                if (!neighbor.Walkable || closedSet.Contains(neighbor))
                    continue;

                int tentativeGScore = gScore[current] + neighbor.Weight;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + Heuristic(neighbor, endNode);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // No valid path found
        return null;
    }

    private GridNode GetLowestFScoreNode(List<GridNode> openSet, Dictionary<GridNode, int> fScore)
    {
        GridNode lowestNode = openSet[0];
        int lowestScore = fScore.ContainsKey(lowestNode) ? fScore[lowestNode] : int.MaxValue;

        foreach (GridNode node in openSet)
        {
            int score = fScore.ContainsKey(node) ? fScore[node] : int.MaxValue;
            if (score < lowestScore)
            {
                lowestNode = node;
                lowestScore = score;
            }
        }
        return lowestNode;
    }

    private List<GridNode> ReconstructPath(Dictionary<GridNode, GridNode> cameFrom, GridNode current)
    {
        List<GridNode> path = new();
        while (cameFrom.ContainsKey(current))
        {
            path.Insert(0, current);
            current = cameFrom[current];
        }
        path.Insert(0, current); // Insert start node
        return path;
    }

    private int Heuristic(GridNode a, GridNode b)
    {
        int dx = Mathf.RoundToInt(Mathf.Abs(a.WorldPosition.x - b.WorldPosition.x));
        int dz = Mathf.RoundToInt(Mathf.Abs(a.WorldPosition.z - b.WorldPosition.z));
        return dx + dz;  // Manhattan distance for grid movement
    }

    private List<GridNode> GetNeighbors(GridNode node)
    {
        List<GridNode> neighbors = new();
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
}
