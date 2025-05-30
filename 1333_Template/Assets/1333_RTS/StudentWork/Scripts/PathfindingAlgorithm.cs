using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathfindingAlgorithm : MonoBehaviour 
{
    public abstract List<GridNode> FindPath(GridManager gridManager, GridNode start, GridNode end, int unitWidth, int unitHeight);
    public abstract List<GridNode> FindPath(GridManager gridManager, Vector3 start, Vector3 end, int unitWidth, int unitHeight);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
